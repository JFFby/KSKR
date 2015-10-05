using System.Text.RegularExpressions;

namespace UI
{
    public static class StringValueHelper
    {
        public static string ProcessValue(string value)
        {
            value = value.Trim();

            if (value == string.Empty) return "0";

            const string patterm = @"\d+\.\d*";
            if (Regex.IsMatch(value, patterm))
            {
                value = value.Replace(".", ",");
            }

            return value;
        }
    }
}
