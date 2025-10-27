using FinancialManagement.Application.Services;
using FinancialManagement.Controllers;
using FinancialManagement.Domain.Entities;
using FinancialManagement.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace FinancialManagement.Tests;

public class CreditCardControllerTests
{
    private readonly Mock<CreditCardService> _creditCardServiceMock;
    private readonly CreditCardController _controller;

    public CreditCardControllerTests()
    {
        var creditCardRepoMock = new Mock<CreditCardRepository>();
        var billRepoMock = new Mock<CreditCardBillRepository>();
        _creditCardServiceMock = new Mock<CreditCardService>(creditCardRepoMock.Object, billRepoMock.Object);
        _controller = new CreditCardController(_creditCardServiceMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithCreditCards()
    {
        // Arrange
        var creditCards = new List<CreditCard>
        {
            new() { Name = "Card1" },
            new() { Name = "Card2" }
        };
        _creditCardServiceMock.Setup(s => s.GetAllAsync()).Returns(creditCards);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(creditCards, okResult.Value);
    }

    [Fact]
    public async Task GetByName_ReturnsOk_WhenCreditCardExists()
    {
        // Arrange
        var cardName = "TestCard";
        var creditCard = new CreditCard { Name = cardName };
        _creditCardServiceMock.Setup(s => s.GetCreditCardByNameAsync(cardName)).Returns(creditCard);

        // Act
        var result = await _controller.GetByName(cardName);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(creditCard, okResult.Value);
    }

    [Fact]
    public async Task GetByName_ReturnsNotFound_WhenCreditCardDoesNotExist()
    {
        // Arrange
        var cardName = "NonExistentCard";
        _creditCardServiceMock.Setup(s => s.GetCreditCardByNameAsync(cardName)).Returns((CreditCard?)null);

        // Act
        var result = await _controller.GetByName(cardName);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Add_ReturnsCreatedAtAction()
    {
        // Arrange
        var cardName = "NewCard";
        _creditCardServiceMock.Setup(s => s.AddAsync(cardName)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Add(cardName);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetByName), createdResult.ActionName);
        Assert.Equal(cardName, createdResult.RouteValues["name"]);
    }

    [Fact]
    public async Task Update_ReturnsNoContent()
    {
        // Arrange
        var creditCard = new CreditCard { Name = "UpdatedCard" };
        _creditCardServiceMock.Setup(s => s.UpdateAsync(creditCard)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(creditCard);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        // Arrange
        var rowKey = Guid.NewGuid();
        _creditCardServiceMock.Setup(s => s.DeleteAsync(rowKey)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(rowKey);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task GetBills_ReturnsOkWithBills()
    {
        // Arrange
        var bills = new List<CreditCardBill>
        {
            new() { PartitionKey = "Card1" },
            new() { PartitionKey = "Card2" }
        };
        _creditCardServiceMock.Setup(s => s.GetBillsAsync()).Returns(bills);

        // Act
        var result = await _controller.GetBills();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(bills, okResult.Value);
    }

    [Fact]
    public async Task GetBillsByCard_ReturnsOkWithBills()
    {
        // Arrange
        var cardName = "TestCard";
        var bills = new List<CreditCardBill>
        {
            new() { PartitionKey = cardName }
        };
        _creditCardServiceMock.Setup(s => s.GetBillsByCardAsync(cardName)).Returns(bills);

        // Act
        var result = await _controller.GetBillsByCard(cardName);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(bills, okResult.Value);
    }
}