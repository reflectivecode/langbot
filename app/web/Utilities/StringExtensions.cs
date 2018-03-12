using System;

namespace LangBot
{
    public static class StringExtensions
    {
        public static string ReturnNullIfEmpty(this string value)
        {
            if (String.IsNullOrEmpty(value)) return null;
            return value;
        }
    }
}
