using FinancialManagement.Application.Services;
using FinancialManagement.Controllers;
using FinancialManagement.Domain.Entities;
using FinancialManagement.Domain.Enum;
using FinancialManagement.Domain.FinancialManagement;
using FinancialManagement.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace FinancialManagement.Tests;

public class FinancialManagementControllerTests
{
    private readonly Mock<FinancialManagementService> _financialManagementServiceMock;
    private readonly FinancialManagementController _controller;

    public FinancialManagementControllerTests()
    {
        var creditCardRepoMock = new Mock<CreditCardRepository>();
        var billRepoMock = new Mock<CreditCardBillRepository>();
        var purchaseRepoMock = new Mock<PurchaseRepository>();
        _financialManagementServiceMock = new Mock<FinancialManagementService>(
            creditCardRepoMock.Object, billRepoMock.Object, purchaseRepoMock.Object);
        _controller = new FinancialManagementController(_financialManagementServiceMock.Object);
    }

    [Fact]
    public async Task GetByDate_ReturnsOk_WhenValidDate()
    {
        // Arrange
        var date = "2023-10-01";
        var parsedDate = DateTime.Parse(date);
        var monthlyBills = new List<MonthlyBill>
        {
            new() { BillName = "Test Bill", Value = 100, Paid = true }
        };
        _financialManagementServiceMock.Setup(s => s.QueryMonthlyBills(parsedDate)).Returns(monthlyBills);

        // Act
        var result = await _controller.GetByDate(date);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(monthlyBills, okResult.Value);
    }

    [Fact]
    public async Task GetByDate_ReturnsBadRequest_WhenInvalidDate()
    {
        // Arrange
        var invalidDate = "invalid-date";

        // Act
        var result = await _controller.GetByDate(invalidDate);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid date format", badRequestResult.Value);
    }

    [Fact]
    public async Task AddPurchase_ReturnsCreatedAtAction_WhenValidPurchase()
    {
        // Arrange
        var purchase = new Purchase { RowKey = "TestPurchase", Timestamp = DateTimeOffset.Now };
        _financialManagementServiceMock.Setup(s => s.AddPurchaseAsync(purchase)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AddPurchase(purchase);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetByDate), createdResult.ActionName);
    }

    [Fact]
    public async Task AddPurchase_ReturnsBadRequest_WhenNullPurchase()
    {
        // Act
        var result = await _controller.AddPurchase(null);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task UpdatePurchase_ReturnsCreatedAtAction_WhenValidPurchase()
    {
        // Arrange
        var purchase = new Purchase { RowKey = "TestPurchase", Timestamp = DateTimeOffset.Now };
        _financialManagementServiceMock.Setup(s => s.UpdatePurchaseAsync(purchase)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdatePurchase(purchase);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetByDate), createdResult.ActionName);
    }

    [Fact]
    public async Task UpdatePurchase_ReturnsBadRequest_WhenNullPurchase()
    {
        // Act
        var result = await _controller.UpdatePurchase(null);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task DeletePurchase_ReturnsNoContent()
    {
        // Arrange
        var rowKey = Guid.NewGuid();
        _financialManagementServiceMock.Setup(s => s.DeletePurchaseAsync(rowKey)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeletePurchase(rowKey);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task AddPurchaseToCreditCard_ReturnsCreatedAtAction_WhenValid()
    {
        // Arrange
        var creditCard = "TestCard";
        var purchase = new Purchase { RowKey = "TestPurchase", Timestamp = DateTimeOffset.Now };
        _financialManagementServiceMock.Setup(s => s.AddPurchaseToCreditCardAsync(creditCard, purchase)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AddPurchaseToCreditCard(creditCard, purchase);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetByDate), createdResult.ActionName);
    }

    [Fact]
    public async Task AddPurchaseToCreditCard_ReturnsBadRequest_WhenNullPurchase()
    {
        // Arrange
        var creditCard = "TestCard";

        // Act
        var result = await _controller.AddPurchaseToCreditCard(creditCard, null);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task AddPurchaseToCreditCard_ReturnsBadRequest_WhenEmptyCreditCard()
    {
        // Arrange
        var purchase = new Purchase();

        // Act
        var result = await _controller.AddPurchaseToCreditCard("", purchase);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task AddCreditCardBill_ReturnsBadRequest_WhenFileIsNotCsv()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.ContentType).Returns("text/plain");
        fileMock.Setup(f => f.Length).Returns(100);

        // Act
        var result = await _controller.AddCreditCardBill(CsvBillType.Xp, DateTimeOffset.Now, "TestCard", fileMock.Object);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("File is empty or not a CSV file", badRequestResult.Value);
    }

    [Fact]
    public async Task AddCreditCardBill_ReturnsBadRequest_WhenFileIsEmpty()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.ContentType).Returns("text/csv");
        fileMock.Setup(f => f.Length).Returns(0);

        // Act
        var result = await _controller.AddCreditCardBill(CsvBillType.Xp, DateTimeOffset.Now, "TestCard", fileMock.Object);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("File is empty or not a CSV file", badRequestResult.Value);
    }

    [Fact]
    public async Task AddCreditCardBill_ReturnsNoContent_ForPersonalBill()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.ContentType).Returns("text/csv");
        fileMock.Setup(f => f.Length).Returns(100);
        var month = DateTimeOffset.Now;
        _financialManagementServiceMock.Setup(s => s.AddPersonalAccountSheetCsvBillToPurchases(fileMock.Object, month)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AddCreditCardBill(CsvBillType.Personal, month, null, fileMock.Object);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task AddCreditCardBill_ReturnsBadRequest_WhenCardNameNullForNonPersonal()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.ContentType).Returns("text/csv");
        fileMock.Setup(f => f.Length).Returns(100);

        // Act
        var result = await _controller.AddCreditCardBill(CsvBillType.Xp, DateTimeOffset.Now, null, fileMock.Object);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Credit card name cannot be null or empty", badRequestResult.Value);
    }

    [Fact]
    public async Task AddCreditCardBill_ReturnsNoContent_ForCreditCardBill()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.ContentType).Returns("text/csv");
        fileMock.Setup(f => f.Length).Returns(100);
        var month = DateTimeOffset.Now;
        var cardName = "TestCard";
        _financialManagementServiceMock.Setup(s => s.AddCsvCardBillToCreditCardBill(CsvBillType.Xp, cardName, month, fileMock.Object)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AddCreditCardBill(CsvBillType.Xp, month, cardName, fileMock.Object);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}