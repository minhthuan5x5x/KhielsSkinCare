using System.Globalization;

namespace KhielsSkincare.Extensions
{
    public static class CurrencyExtensions
    {
        public static string ToVnd(this decimal value)
        {
            return value.ToString("C0", CultureInfo.GetCultureInfo("vi-VN"));
        }
    }
}
