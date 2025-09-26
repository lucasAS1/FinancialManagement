using Azure;
using Azure.Data.Tables;

namespace FinancialManagement.Domain.Entities;

public record Purchase() : ITableEntity
{
    public double Cost { get; set; }
    public DateTimeOffset DueDate { get; set; }
    public bool Paid { get; set; }
    
    #region ItableEntity implementation
    
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    /// <summary>
    /// PartitionKey is being used as the unique primary key of purchases
    /// </summary>
    public string PartitionKey { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Rowkey is being used as the name of the purchase
    /// </summary>
    public string RowKey { get; set; }
    
    #endregion
}