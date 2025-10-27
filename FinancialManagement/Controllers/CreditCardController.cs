using FinancialManagement.Application.Services;
using FinancialManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CreditCardController : ControllerBase
{
    private readonly CreditCardService _creditCardService;

    public CreditCardController(CreditCardService creditCardService)
    {
        _creditCardService = creditCardService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CreditCard>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        var creditCards = _creditCardService.GetAllAsync();
        return Ok(creditCards);
    }

    [HttpGet("{name}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreditCard))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByName(string name)
    {
        var creditCard = _creditCardService.GetCreditCardByNameAsync(name);
        if (creditCard == null)
        {
            return NotFound();
        }

        return Ok(creditCard);
    }

    [HttpPost("{cardName}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add(string cardName)
    {
        await _creditCardService.AddAsync(cardName);
        return CreatedAtAction(nameof(GetByName), new { name = cardName }, null);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromBody] CreditCard creditCard)
    {
        await _creditCardService.UpdateAsync(creditCard);
        return NoContent();
    }

    [HttpDelete("{rowKey}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid rowKey)
    {
        await _creditCardService.DeleteAsync(rowKey);
        return NoContent();
    }

    [HttpGet("bills")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CreditCardBill>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBills()
    {
        var bills = _creditCardService.GetBillsAsync();
        return Ok(bills);
    }

    [HttpGet("{cardName}/bills")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CreditCardBill>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBillsByCard(string cardName)
    {
        var bills = _creditCardService.GetBillsByCardAsync(cardName);
        return Ok(bills);
    }
}