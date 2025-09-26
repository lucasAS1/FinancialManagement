using FinancialManagement.Domain.Entities;
using FinancialManagement.Infrastructure.Repositories;

namespace FinancialManagement.Application.Services;

public class CreditCardService
{
    private readonly CreditCardRepository _creditCardRepository;

    public CreditCardService(CreditCardRepository creditCardRepository)
    {
        _creditCardRepository = creditCardRepository;
    }

    public IEnumerable<CreditCard> GetAllAsync()
    {
        return _creditCardRepository.GetAll();
    }

    public CreditCard? GetCreditCardByNameAsync(string name)
    {
        return _creditCardRepository.GetByKey(name);
    }

    public async Task AddAsync(string cardName)
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

    public async Task UpdateAsync(CreditCard creditCard)
    {
        await _creditCardRepository.Update(creditCard);
    }

    public async Task DeleteAsync(Guid rowKey)
    {
        await _creditCardRepository.Delete(rowKey);
    }
}