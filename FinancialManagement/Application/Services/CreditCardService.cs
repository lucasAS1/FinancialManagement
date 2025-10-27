using FinancialManagement.Domain.Entities;
using FinancialManagement.Infrastructure.Repositories;

namespace FinancialManagement.Application.Services;

public class CreditCardService
{
    private readonly CreditCardRepository _creditCardRepository;
    private readonly CreditCardBillRepository _creditCardBillRepository;

    public CreditCardService(CreditCardRepository creditCardRepository, CreditCardBillRepository creditCardBillRepository)
    {
        _creditCardRepository = creditCardRepository;
        _creditCardBillRepository = creditCardBillRepository;
    }

    public virtual IEnumerable<CreditCard> GetAllAsync()
    {
        return _creditCardRepository.GetAll();
    }

    public virtual CreditCard? GetCreditCardByNameAsync(string name)
    {
        return _creditCardRepository.GetByKey(name);
    }

    public virtual async Task AddAsync(string cardName)
    {
        var creditCard = new CreditCard
        {
            RowKey = Guid.NewGuid()
                .ToString(),
            IsDeleted = false,
            PartitionKey = cardName,
            Name = cardName
        };

        await _creditCardRepository.Add(creditCard);
    }

    public virtual async Task UpdateAsync(CreditCard creditCard)
    {
        await _creditCardRepository.Update(creditCard);
    }

    public virtual async Task DeleteAsync(Guid rowKey)
    {
        await _creditCardRepository.Delete(rowKey);
    }

    public virtual IEnumerable<CreditCardBill> GetBillsAsync()
    {
        return _creditCardBillRepository.GetAll();
    }

    public virtual IEnumerable<CreditCardBill> GetBillsByCardAsync(string cardName)
    {
        return _creditCardBillRepository.GetAll().Where(b => b.PartitionKey == cardName);
    }
}