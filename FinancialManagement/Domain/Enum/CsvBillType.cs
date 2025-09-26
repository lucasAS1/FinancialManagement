using System.ComponentModel;

namespace FinancialManagement.Domain.Enum;

public enum CsvBillType
{
    [Description("None")]
    None = 0,

    [Description("Xp")]
    Xp = 1,

    [Description("C6")]
    C6 = 2,

    [Description("Santander")]
    Santander = 3,

    [Description("Personal")]
    Personal = 4
}