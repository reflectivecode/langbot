using System;

namespace LangBot
{
    public static class Utilities
    {
        private static readonly char[] SEMICOLON = new[] { ':' };

        public static (string action, Guid? guid) ParseActionWithGuid(string action)
        {
            if (String.IsNullOrEmpty(action)) return (null, null);
            var array = action.Split(SEMICOLON, 2);
            if (array.Length == 0) return (null, null);
            if (array.Length == 1 || !Guid.TryParse(array[1], out var guid)) return (array[0], null);
            return (array[0], guid);
        }
    }
}
