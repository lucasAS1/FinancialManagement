using Azure;
using Azure.Data.Tables;

namespace FinancialManagement.Domain.Entities;

public record CreditCard : ITableEntity
{
    public string Name { get; set; }
    public string Flag { get; set; }
    public bool IsDeleted { get; set; } = false;

    #region ITableEntity implementation
    
    /// <summary>
    /// PartitionKey is being used as the credit card name
    /// </summary>
    public string PartitionKey { get; set; }
    
    /// <summary>
    /// RowKey is being used as the unique primary key of credit cards
    /// </summary>
    public string RowKey { get; set; }
    
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    
    #endregion
}