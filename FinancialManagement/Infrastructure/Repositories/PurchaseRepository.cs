using Azure;
using Azure.Data.Tables;
using FinancialManagement.Domain.Entities;

namespace FinancialManagement.Infrastructure.Repositories;

public class PurchaseRepository
{
    private readonly TableClient _tableClient;

    public PurchaseRepository(IConfiguration configuration)
    {
        var storageAccountConnectionString = configuration.GetConnectionString("StorageAccount");
        var serviceClient = new TableServiceClient(storageAccountConnectionString);
        _tableClient = serviceClient.GetTableClient("Purchases");
        _tableClient.CreateIfNotExists();
    }

    // Parameterless constructor for testing
    protected PurchaseRepository()
    {
        _tableClient = null!;
    }

    public virtual IEnumerable<Purchase> GetAllFromMonth(DateTime month)
    {
        Pageable<Purchase> query = _tableClient.Query<Purchase>(filter: $"DueDate ge datetime'{month.ToString("yyyy-MM-dd")}' and DueDate lt datetime'{month.AddMonths(1).ToString("yyyy-MM-dd")}'");

        var result = new List<Purchase>();

        foreach (var purchase in query)
        {
            result.Add(purchase);
        }

        return result;
    }

    public virtual async Task<Purchase?> GetByIdAsync(Guid id)
    {
        var response = await _tableClient.GetEntityAsync<Purchase>(id.ToString(), id.ToString());

        return response.Value;
    }

    public virtual async Task AddAsync(Purchase purchase)
    {
        await _tableClient.AddEntityAsync(purchase);
    }

    public virtual async Task UpdateAsync(Purchase purchase)
    {
        await _tableClient.UpdateEntityAsync(purchase, ETag.All, TableUpdateMode.Replace);
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        await _tableClient.DeleteEntityAsync(id.ToString(), id.ToString());
    }
}