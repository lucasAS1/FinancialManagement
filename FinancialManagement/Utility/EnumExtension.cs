using System.ComponentModel;
using System.Reflection;

namespace FinancialManagement.Utility;

public static class EnumExtension
{
    public static string GetDescription(this Enum value)
    {
        FieldInfo field = value.GetType().GetField(value.ToString());
        DescriptionAttribute attribute = (DescriptionAttribute)field.GetCustomAttribute(typeof(DescriptionAttribute));

        return attribute.Description;
    }
    
    public static TEnum GetEnumFromDescription<TEnum>(this string description) where TEnum : Enum
    {
        foreach (var field in typeof(TEnum).GetFields())
        {
            if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is
                DescriptionAttribute attribute)
            {
                if (string.Equals(attribute.Description, description,
                        true ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                    return (TEnum) field.GetValue(null);
            }
            else
            {
                if (string.Equals(field.Name, description,
                        true ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                    return (TEnum) field.GetValue(null);
            }
        }

        return default;
    }
}