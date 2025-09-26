using CsvHelper.Configuration.Attributes;

namespace FinancialManagement.Domain.CsvModels;

[Delimiter(";")]
public record PersonalBill
{
    public string Nome { get; set; }
    public string Valor { get; set; }
    
    [Name("pago?")]
    [Optional]
    public string Pago { get; set; }
}