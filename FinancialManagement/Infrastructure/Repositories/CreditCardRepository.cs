using Azure;
using Azure.Data.Tables;
using FinancialManagement.Domain.Entities;

namespace FinancialManagement.Infrastructure.Repositories;

public class CreditCardRepository
{
    private readonly TableClient _tableClient;

    public CreditCardRepository(IConfiguration configuration)
    {
        var storageAccountConnectionString = configuration.GetConnectionString("StorageAccount");
        var serviceClient = new TableServiceClient(storageAccountConnectionString);
        _tableClient = serviceClient.GetTableClient("CreditCards");
        _tableClient.CreateIfNotExists();
    }

    // Parameterless constructor for testing
    protected CreditCardRepository()
    {
        _tableClient = null!;
    }

    public virtual IEnumerable<CreditCard> GetAll()
    {
        return _tableClient.Query<CreditCard>(cc => !cc.IsDeleted).ToList();
    }

    public virtual CreditCard? GetById(Guid id)
    {
        return _tableClient.Query<CreditCard>(cc => cc.RowKey == id.ToString() && !cc.IsDeleted).FirstOrDefault();
    }

    public virtual CreditCard? GetByKey(string key)
    {
        return _tableClient.Query<CreditCard>(cc => cc.PartitionKey == key && !cc.IsDeleted).FirstOrDefault();
    }

    public virtual async Task Add(CreditCard creditCard)
    {
        await _tableClient.AddEntityAsync(creditCard);
    }

    public virtual async Task Update(CreditCard creditCard)
    {
        await _tableClient.UpdateEntityAsync(creditCard, ETag.All, TableUpdateMode.Replace);
    }

    public virtual async Task Delete(Guid id)
    {
        var creditCard = GetById(id);

        if (creditCard != null)
        {
            creditCard.IsDeleted = true;
            await Update(creditCard);
        }
    }
}