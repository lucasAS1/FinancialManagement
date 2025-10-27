using FinancialManagement.Application.Services;
using FinancialManagement.Domain.Entities;
using FinancialManagement.Domain.FinancialManagement;
using FinancialManagement.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Moq;

namespace FinancialManagement.Tests;

public class FinancialManagementServiceTests
{
    private readonly Mock<CreditCardRepository> _creditCardRepositoryMock;
    private readonly Mock<CreditCardBillRepository> _creditCardBillRepositoryMock;
    private readonly Mock<PurchaseRepository> _purchaseRepositoryMock;
    private readonly FinancialManagementService _service;

    public FinancialManagementServiceTests()
    {
        _creditCardRepositoryMock = new Mock<CreditCardRepository>();
        _creditCardBillRepositoryMock = new Mock<CreditCardBillRepository>();
        _purchaseRepositoryMock = new Mock<PurchaseRepository>();
        _service = new FinancialManagementService(
            _creditCardRepositoryMock.Object,
            _creditCardBillRepositoryMock.Object,
            _purchaseRepositoryMock.Object);
    }

    [Fact]
    public void QueryMonthlyBills_ReturnsMonthlyBills()
    {
        // Arrange
        var month = new DateTime(2023, 10, 1);
        var creditCards = new List<CreditCard>
        {
            new() { PartitionKey = "Card1", Name = "Credit Card 1" },
            new() { PartitionKey = "Card2", Name = "Credit Card 2" }
        };
        var bill1 = new CreditCardBill
        {
            PartitionKey = "Card1",
            Purchases = new List<Purchase> { new() { Cost = 100 }, new() { Cost = 50 } },
            Paid = true
        };
        var purchases = new List<Purchase>
        {
            new() { RowKey = "Purchase1", Cost = 200, Paid = false }
        };

        _creditCardRepositoryMock.Setup(r => r.GetAll()).Returns(creditCards);
        _creditCardBillRepositoryMock.Setup(r => r.GetById("Card1", "2023-10")).Returns(bill1);
        _creditCardBillRepositoryMock.Setup(r => r.GetById("Card2", "2023-10")).Returns((CreditCardBill?)null);
        _purchaseRepositoryMock.Setup(r => r.GetAllFromMonth(month)).Returns(purchases);

        // Act
        var result = _service.QueryMonthlyBills(month).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        var cardBill = result.First(r => r.BillName == "Credit Card 1");
        Assert.Equal(150, cardBill.Value);
        Assert.True(cardBill.Paid);
        var purchaseBill = result.First(r => r.BillName == "Purchase1");
        Assert.Equal(200, purchaseBill.Value);
        Assert.False(purchaseBill.Paid);
    }

    [Fact]
    public async Task AddPurchaseAsync_AddsPurchase()
    {
        // Arrange
        var purchase = new Purchase { RowKey = "TestPurchase" };
        _purchaseRepositoryMock.Setup(r => r.AddAsync(purchase)).Returns(Task.CompletedTask);

        // Act
        await _service.AddPurchaseAsync(purchase);

        // Assert
        _purchaseRepositoryMock.Verify(r => r.AddAsync(purchase), Times.Once);
    }

    [Fact]
    public async Task UpdatePurchaseAsync_UpdatesPurchase()
    {
        // Arrange
        var purchase = new Purchase { RowKey = "TestPurchase" };
        _purchaseRepositoryMock.Setup(r => r.UpdateAsync(purchase)).Returns(Task.CompletedTask);

        // Act
        await _service.UpdatePurchaseAsync(purchase);

        // Assert
        _purchaseRepositoryMock.Verify(r => r.UpdateAsync(purchase), Times.Once);
    }

    [Fact]
    public async Task DeletePurchaseAsync_DeletesPurchase()
    {
        // Arrange
        var rowKey = Guid.NewGuid();
        _purchaseRepositoryMock.Setup(r => r.DeleteAsync(rowKey)).Returns(Task.CompletedTask);

        // Act
        await _service.DeletePurchaseAsync(rowKey);

        // Assert
        _purchaseRepositoryMock.Verify(r => r.DeleteAsync(rowKey), Times.Once);
    }

    [Fact]
    public async Task AddPurchaseToCreditCardAsync_AddsToExistingBill()
    {
        // Arrange
        var creditCardName = "TestCard";
        var creditCard = new CreditCard { PartitionKey = creditCardName, RowKey = "card-id" };
        var purchase = new Purchase { DueDate = new DateTime(2023, 10, 15) };
        var existingBill = new CreditCardBill
        {
            PartitionKey = creditCardName,
            RowKey = "2023-10",
            Purchases = new List<Purchase>()
        };

        _creditCardRepositoryMock.Setup(r => r.GetByKey(creditCardName)).Returns(creditCard);
        _creditCardBillRepositoryMock.Setup(r => r.GetById(It.IsAny<string>(), It.IsAny<string>())).Returns(existingBill);
        _creditCardBillRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<CreditCardBill>())).Returns(Task.CompletedTask);

        // Act
        await _service.AddPurchaseToCreditCardAsync(creditCardName, purchase);

        // Assert
        _creditCardBillRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<CreditCardBill>()), Times.Once);
    }

    [Fact]
    public async Task AddPurchaseToCreditCardAsync_CreatesNewBill_WhenNoExistingBill()
    {
        // Arrange
        var creditCardName = "TestCard";
        var creditCard = new CreditCard { PartitionKey = creditCardName, RowKey = "card-id" };
        var purchase = new Purchase { DueDate = new DateTime(2023, 10, 15) };

        _creditCardRepositoryMock.Setup(r => r.GetByKey(creditCardName)).Returns(creditCard);
        _creditCardBillRepositoryMock.Setup(r => r.GetById(creditCard.RowKey, "2023-10")).Returns((CreditCardBill?)null);
        _creditCardBillRepositoryMock.Setup(r => r.AddAsync(It.IsAny<CreditCardBill>())).Returns(Task.CompletedTask);

        // Act
        await _service.AddPurchaseToCreditCardAsync(creditCardName, purchase);

        // Assert
        _creditCardBillRepositoryMock.Verify(r => r.AddAsync(It.IsAny<CreditCardBill>()), Times.Once);
    }

    [Fact]
    public async Task AddPurchaseToCreditCardAsync_ThrowsException_WhenCreditCardNotFound()
    {
        // Arrange
        var creditCardName = "NonExistentCard";
        var purchase = new Purchase();
        _creditCardRepositoryMock.Setup(r => r.GetByKey(creditCardName)).Returns((CreditCard?)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.AddPurchaseToCreditCardAsync(creditCardName, purchase));
    }
}