using System.Runtime.Serialization;
using System.Text.Json;
using Azure;
using Azure.Data.Tables;

namespace FinancialManagement.Domain.Entities;

public record CreditCardBill() : ITableEntity
{
    public DateTimeOffset DueDate { get; set; }
    public string SerializedPurchases { get; set; }
    
    [IgnoreDataMember]
    public List<Purchase> Purchases
    {
        get => string.IsNullOrEmpty(SerializedPurchases) ? new List<Purchase>() : JsonSerializer.Deserialize<List<Purchase>>(SerializedPurchases) ?? [];
        set => SerializedPurchases = JsonSerializer.Serialize(value);
    }
    
    #region ITableEntity implementation
    
    /// <summary>
    /// PartitionKey is being used as a foreign key to the CreditCard entity
    /// </summary>
    public string PartitionKey { get; set; }

    /// <summary>
    /// RowKey is being used as the month of the credit card bill
    /// </summary>
    public string RowKey { get; set; } = Guid.NewGuid().ToString();
    
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    public bool Paid { get; set; }
    
    #endregion
}