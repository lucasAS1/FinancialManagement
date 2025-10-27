using FinancialManagement.Application.Services;
using FinancialManagement.Domain.Entities;
using FinancialManagement.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Moq;

namespace FinancialManagement.Tests;

public class CreditCardServiceTests
{
    private readonly Mock<CreditCardRepository> _creditCardRepositoryMock;
    private readonly Mock<CreditCardBillRepository> _creditCardBillRepositoryMock;
    private readonly CreditCardService _service;

    public CreditCardServiceTests()
    {
        _creditCardRepositoryMock = new Mock<CreditCardRepository>();
        _creditCardBillRepositoryMock = new Mock<CreditCardBillRepository>();
        _service = new CreditCardService(_creditCardRepositoryMock.Object, _creditCardBillRepositoryMock.Object);
    }

    [Fact]
    public void GetAllAsync_ReturnsAllCreditCards()
    {
        // Arrange
        var expectedCards = new List<CreditCard>
        {
            new() { Name = "Card1", PartitionKey = "Card1" },
            new() { Name = "Card2", PartitionKey = "Card2" }
        };
        _creditCardRepositoryMock.Setup(r => r.GetAll()).Returns(expectedCards);

        // Act
        var result = _service.GetAllAsync();

        // Assert
        Assert.Equal(expectedCards, result);
    }

    [Fact]
    public void GetCreditCardByNameAsync_ReturnsCreditCard_WhenExists()
    {
        // Arrange
        var cardName = "TestCard";
        var expectedCard = new CreditCard { Name = cardName, PartitionKey = cardName };
        _creditCardRepositoryMock.Setup(r => r.GetByKey(cardName)).Returns(expectedCard);

        // Act
        var result = _service.GetCreditCardByNameAsync(cardName);

        // Assert
        Assert.Equal(expectedCard, result);
    }

    [Fact]
    public void GetCreditCardByNameAsync_ReturnsNull_WhenNotExists()
    {
        // Arrange
        var cardName = "NonExistentCard";
        _creditCardRepositoryMock.Setup(r => r.GetByKey(cardName)).Returns((CreditCard?)null);

        // Act
        var result = _service.GetCreditCardByNameAsync(cardName);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_CreatesNewCreditCard()
    {
        // Arrange
        var cardName = "NewCard";
        _creditCardRepositoryMock.Setup(r => r.Add(It.IsAny<CreditCard>())).Returns(Task.CompletedTask);

        // Act
        await _service.AddAsync(cardName);

        // Assert
        _creditCardRepositoryMock.Verify(r => r.Add(It.Is<CreditCard>(c =>
            c.Name == cardName &&
            c.PartitionKey == cardName &&
            !c.IsDeleted)), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesCreditCard()
    {
        // Arrange
        var creditCard = new CreditCard { Name = "UpdatedCard" };
        _creditCardRepositoryMock.Setup(r => r.Update(creditCard)).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(creditCard);

        // Assert
        _creditCardRepositoryMock.Verify(r => r.Update(creditCard), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_DeletesCreditCard()
    {
        // Arrange
        var rowKey = Guid.NewGuid();
        _creditCardRepositoryMock.Setup(r => r.Delete(rowKey)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(rowKey);

        // Assert
        _creditCardRepositoryMock.Verify(r => r.Delete(rowKey), Times.Once);
    }

    [Fact]
    public void GetBillsAsync_ReturnsAllBills()
    {
        // Arrange
        var expectedBills = new List<CreditCardBill>
        {
            new() { PartitionKey = "Card1" },
            new() { PartitionKey = "Card2" }
        };
        _creditCardBillRepositoryMock.Setup(r => r.GetAll()).Returns(expectedBills);

        // Act
        var result = _service.GetBillsAsync();

        // Assert
        Assert.Equal(expectedBills, result);
    }

    [Fact]
    public void GetBillsByCardAsync_ReturnsBillsForSpecificCard()
    {
        // Arrange
        var cardName = "TestCard";
        var allBills = new List<CreditCardBill>
        {
            new() { PartitionKey = cardName },
            new() { PartitionKey = "OtherCard" },
            new() { PartitionKey = cardName }
        };
        _creditCardBillRepositoryMock.Setup(r => r.GetAll()).Returns(allBills);

        // Act
        var result = _service.GetBillsByCardAsync(cardName);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Equal(cardName, b.PartitionKey));
    }
}