using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace FinancialManagement.Utility;

public static class CsvUtilities
{
    public static IEnumerable<T> Deserialize<T>(string base64Data, string delimiter = ",")
    {
        byte[] data = Convert.FromBase64String(base64Data);
        using MemoryStream stream = new(data);
        using StreamReader reader = new(stream);
        
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = delimiter,
        });
        
        var records = csv.GetRecords<T>().ToList();

        return records;
    }
}