using CsvHelper.Configuration.Attributes;

namespace FinancialManagement.Domain.CsvModels;

[Delimiter(";")]
public class XpBill
{
    public string Data { get; set; }
    public string Estabelecimento { get; set; }
    public string Portador { get; set; }
    public string Valor { get; set; }

    public int ParcelasRestantes
    {
        get
        {
            if (Parcela == "-") return 0;
            var parts = Parcela.Split("de", StringSplitOptions.RemoveEmptyEntries);
            (var parcela, var total) = (int.Parse(parts[0]), int.Parse(parts[1]));
            return total - parcela;
        }
    }

    public string Parcela { get; set; }
}