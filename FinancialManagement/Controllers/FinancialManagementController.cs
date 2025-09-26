using System.ComponentModel.DataAnnotations;
using FinancialManagement.Application.Services;
using FinancialManagement.Domain.Entities;
using FinancialManagement.Domain.Enum;
using FinancialManagement.Domain.FinancialManagement;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FinancialManagementController : ControllerBase
{
    private readonly FinancialManagementService _financialManagementService;

    public FinancialManagementController(FinancialManagementService financialManagementService)
    {
        _financialManagementService = financialManagementService;
    }

    [HttpGet("{date}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<MonthlyBill>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> GetByDate(string date)
    {
        if (!DateTime.TryParse(date, out var parsedDate))
        {
            return Task.FromResult<IActionResult>(BadRequest("Invalid date format"));
        }

        var financialData = _financialManagementService.QueryMonthlyBills(parsedDate);
        return Task.FromResult<IActionResult>(Ok(financialData));
    }

    [HttpPost("purchase")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddPurchase([FromBody] Purchase purchase)
    {
        if (purchase == null)
        {
            return BadRequest();
        }

        await _financialManagementService.AddPurchaseAsync(purchase);
        return CreatedAtAction(nameof(GetByDate), new { date = purchase.Timestamp }, null);
    }
    
    [HttpPut("purchase")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePurchase([FromBody] Purchase purchase)
    {
        if (purchase == null)
        {
            return BadRequest();
        }

        await _financialManagementService.UpdatePurchaseAsync(purchase);
        return CreatedAtAction(nameof(GetByDate), new { date = purchase.Timestamp }, null);
    }
    
    [HttpDelete("purchase")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeletePurchase([FromQuery] Guid rowKey)
    {
        await _financialManagementService.DeletePurchaseAsync(rowKey);
        
        return NoContent();
    }

    [HttpPost("credit-card/{creditCard}/purchase")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddPurchaseToCreditCard(string creditCard, [FromBody] Purchase purchase)
    {
        if (purchase == null || string.IsNullOrWhiteSpace(creditCard))
        {
            return BadRequest();
        }

        await _financialManagementService.AddPurchaseToCreditCardAsync(creditCard, purchase);
        return CreatedAtAction(nameof(GetByDate), new { date = purchase.Timestamp }, null);
    }
    
    [HttpPost("bill")]
    public async Task<IActionResult> AddCreditCardBill([Required] CsvBillType bank, [Required] DateTimeOffset month, string? cardName, [Required] IFormFile file)
    {
        if (file.ContentType != "text/csv" || file.Length == 0)
        {
            return BadRequest("File is empty or not a CSV file");
        }

        if (bank == CsvBillType.Personal)
        {
            await _financialManagementService.AddPersonalAccountSheetCsvBillToPurchases(file, month);
            return NoContent();
        }

        if (string.IsNullOrEmpty(cardName)) return BadRequest("Credit card name cannot be null or empty");
        
        await _financialManagementService.AddCsvCardBillToCreditCardBill(bank, cardName, month, file);

        return NoContent();
    }
}