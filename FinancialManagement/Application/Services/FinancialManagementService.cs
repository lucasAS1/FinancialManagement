using System.Globalization;
using System.Text.Json;
using FinancialManagement.Domain.CsvModels;
using FinancialManagement.Domain.Entities;
using FinancialManagement.Domain.Enum;
using FinancialManagement.Domain.FinancialManagement;
using FinancialManagement.Infrastructure.Repositories;
using FinancialManagement.Utility;

namespace FinancialManagement.Application.Services;

public class FinancialManagementService
{
    private readonly CreditCardRepository _creditCardRepository;
    private readonly CreditCardBillRepository _creditCardBillRepository;
    private readonly PurchaseRepository _purchaseRepository;

    public FinancialManagementService(
        CreditCardRepository creditCardRepository,
        CreditCardBillRepository creditCardBillRepository,
        PurchaseRepository purchaseRepository)
    {
        _creditCardRepository = creditCardRepository;
        _creditCardBillRepository = creditCardBillRepository;
        _purchaseRepository = purchaseRepository;
    }

    public virtual IEnumerable<MonthlyBill> QueryMonthlyBills(DateTime month)
    {
        var creditCards = _creditCardRepository.GetAll();
        var monthlyBills = new List<MonthlyBill>();

        foreach (var creditCard in creditCards)
        {
            var bill = _creditCardBillRepository.GetById(creditCard.PartitionKey, month.ToString("yyyy-MM"));

            if (bill != null)
            {
                var billName = creditCards.First(x => x.PartitionKey == bill.PartitionKey).Name;

                var monthlyBill = new MonthlyBill
                {
                    BillName = billName,
                    Value = bill.Purchases.Sum(p => p.Cost),
                    Paid = bill.Paid
                };

                monthlyBills.Add(monthlyBill);
            }
        }

        var purchases = _purchaseRepository.GetAllFromMonth(month);

        foreach (var purchase in purchases)
        {
            var monthlyBill = new MonthlyBill
            {
                BillName = purchase.RowKey,
                Value = purchase.Cost,
                Paid = purchase.Paid
            };

            monthlyBills.Add(monthlyBill);
        }

        return monthlyBills;
    }

    public virtual async Task AddPurchaseAsync(Purchase purchase)
    {
        await _purchaseRepository.AddAsync(purchase);
    }

    public virtual async Task UpdatePurchaseAsync(Purchase purchase)
    {
        await _purchaseRepository.UpdateAsync(purchase);
    }

    public virtual async Task DeletePurchaseAsync(Guid rowKey)
    {
        await _purchaseRepository.DeleteAsync(rowKey);
    }

    public virtual async Task AddPurchaseToCreditCardAsync(string creditCardName, Purchase purchase)
    {
        var creditCard = _creditCardRepository.GetByKey(creditCardName);

        if (creditCard == null)
        {
            throw new Exception("Credit card not found");
        }

        var bill = _creditCardBillRepository.GetById(creditCard.RowKey, purchase.DueDate.ToString("yyyy-MM"));

        if (bill == null)
        {
            bill = new CreditCardBill
            {
                DueDate = new DateTime(purchase.DueDate.Year, purchase.DueDate.Month, 1),
                Purchases = [purchase],
                PartitionKey = creditCard.PartitionKey,
                RowKey = Guid.NewGuid().ToString()
            };

            await _creditCardBillRepository.AddAsync(bill);

            return;
        }

        bill.Purchases.Add(purchase);
        await _creditCardBillRepository.UpdateAsync(bill);
    }

    public virtual async Task AddCsvCardBillToCreditCardBill(CsvBillType bank, string creditCardName, DateTimeOffset month,
        IFormFile file)
    {
        var creditCard = _creditCardRepository.GetByKey(creditCardName);

        if (creditCard == null)
        {
            throw new Exception("Credit card not found");
        }

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        var base64Data = Convert.ToBase64String(memoryStream.ToArray());

        var creditCardBill = new CreditCardBill
        {
            DueDate = month,
            PartitionKey = creditCardName,
            RowKey = month.ToString("yyyy-MM"),
            Paid = false
        };

        switch (bank)
        {
            case CsvBillType.Xp:
                var xpBills = CsvUtilities.Deserialize<XpBill>(base64Data, ";");
                var currentPurchases = new List<Purchase>();

                foreach (var p in xpBills.Where(x => !x.Valor.Contains('-')))
                {
                    var currentPurchase = new Purchase
                    {
                        Cost = double.Parse(p.Valor.Replace("R$ ", "").Replace(',', '.')),
                        DueDate = DateTimeOffset.ParseExact(p.Data, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        Paid = false,
                        RowKey = p.Estabelecimento + Guid.NewGuid(),
                        PartitionKey = Guid.NewGuid().ToString()
                    };

                    currentPurchases.Add(currentPurchase);

                    if (p.ParcelasRestantes > 0)
                    {
                        await CreateRemainingInstallmentsAsync(creditCard.PartitionKey, p, month, currentPurchase.Cost);
                    }
                }

                creditCardBill.Purchases = currentPurchases;
                break;
            case CsvBillType.C6:
                throw new NotImplementedException();
            case CsvBillType.Santander:
                throw new NotImplementedException();
            default: throw new KeyNotFoundException("Invalid bank");
        }

        await _creditCardBillRepository.UpdateAsync(creditCardBill);
    }

    public virtual async Task AddPersonalAccountSheetCsvBillToPurchases(IFormFile file, DateTimeOffset month)
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        var base64Data = Convert.ToBase64String(memoryStream.ToArray());

        var purchases = CsvUtilities.Deserialize<PersonalBill>(base64Data, ";")
            .Where(x =>
                !string.IsNullOrEmpty(x.Nome) || !string.IsNullOrEmpty(x.Valor))
            .ToList();

        foreach (var purchase in purchases)
        {
            var bill = new Purchase
            {
                Cost = double.Parse(purchase.Valor, NumberStyles.Currency, CultureInfo.GetCultureInfo("pt-BR")),
                DueDate = month,
                Paid = purchase.Pago == "Sim",
                RowKey = purchase.Nome,
                PartitionKey = Guid.NewGuid().ToString()
            };

            await _purchaseRepository.AddAsync(bill);
        }
    }

    private async Task CreateRemainingInstallmentsAsync(string creditCardPartitionKey, XpBill originalBill,
        DateTimeOffset month, double installmentValue)
    {
        var parts = originalBill.Parcela.Split("de", StringSplitOptions.RemoveEmptyEntries);
        var monthString = month.ToString("yyyy-MM");
        var totalInstallments = int.Parse(parts[1].Trim());

        for (int i = 1; i <= originalBill.ParcelasRestantes; i++)
        {
            var installmentMonth = month.AddMonths(i);
            var billRowKey = installmentMonth.ToString("yyyy-MM");

            var futureBill = _creditCardBillRepository.GetById(creditCardPartitionKey, billRowKey);
            
            if (futureBill == null)
            {
                futureBill = new CreditCardBill
                {
                    DueDate = new DateTime(installmentMonth.Year, installmentMonth.Month, 1),
                    PartitionKey = creditCardPartitionKey,
                    Purchases = new List<Purchase>(),
                    SerializedPurchases = "[]",
                    RowKey = billRowKey,
                    Paid = false
                };
            }
            
            var purchases = JsonSerializer.Deserialize<List<Purchase>>(futureBill.SerializedPurchases);

            if (!purchases.Any(x =>
                    x.RowKey == $"{originalBill.Estabelecimento} {originalBill.Valor} ({i} de {totalInstallments})"))
            {
                var installmentPurchase = new Purchase
                {
                    Cost = installmentValue,
                    DueDate = installmentMonth,
                    Paid = false,
                    RowKey = $"{originalBill.Estabelecimento} {originalBill.Valor} ({i} de {totalInstallments})",
                    PartitionKey = Guid.NewGuid().ToString()
                };

                purchases.Add(installmentPurchase);
            }

            futureBill.Purchases = purchases;

            await _creditCardBillRepository.UpdateAsync(futureBill);
        }
    }
}