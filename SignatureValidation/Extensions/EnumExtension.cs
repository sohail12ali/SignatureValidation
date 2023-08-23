using System.ComponentModel;
using System.Reflection;

namespace SignatureValidation.Extensions
{
    public static class EnumExtension
    {
        public static string GetDesc(this Enum value)
        {
            FieldInfo? fi = value?.GetType().GetField(value.ToString());

            if (fi?.GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[] attributes && attributes.Length > 0)
            {
                return attributes[0].Description;
            }

            return value?.ToString() ?? string.Empty;
        }
    }
}