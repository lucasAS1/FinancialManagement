using Azure;
using Azure.Data.Tables;
using FinancialManagement.Domain.Entities;

namespace FinancialManagement.Infrastructure.Repositories;

public class CreditCardBillRepository
{
    private readonly TableClient _tableClient;

    public CreditCardBillRepository(IConfiguration configuration)
    {
        var storageAccountConnectionString = configuration.GetConnectionString("StorageAccount");
        var serviceClient = new TableServiceClient(storageAccountConnectionString);
        _tableClient = serviceClient.GetTableClient("CreditCardBills");
        _tableClient.CreateIfNotExists();
    }

    // Parameterless constructor for testing
    protected CreditCardBillRepository()
    {
        _tableClient = null!;
    }

    public virtual IEnumerable<CreditCardBill> GetAll()
    {
        return _tableClient.Query<CreditCardBill>().ToList();
    }

    public virtual CreditCardBill? GetById(string partitionKey, string rowKey)
    {
        return _tableClient.Query<CreditCardBill>(ccb => ccb.PartitionKey == partitionKey && ccb.RowKey == rowKey).FirstOrDefault();
    }

    public virtual async Task AddAsync(CreditCardBill creditCardBill)
    {
        await _tableClient.AddEntityAsync(creditCardBill);
    }

    public virtual async Task UpdateAsync(CreditCardBill creditCardBill)
    {
        await _tableClient.UpsertEntityAsync(creditCardBill, TableUpdateMode.Replace);
    }

    public virtual async Task DeleteAsync(string partitionKey, string rowKey)
    {
        await _tableClient.DeleteEntityAsync(partitionKey, rowKey);
    }
}