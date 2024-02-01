namespace InfoGatherer.api.Helpers
{
    public static class StringExtensions
    {
        // ToCamelCase(): Konwertuje string na notację camelCase.
        // IsNumeric(): Sprawdza, czy string jest liczbą.
        // Reverse(): Odwraca string.
        // RemoveWhitespace(): Usuwa wszystkie białe znaki ze stringu.
        // ContainsAny(): Sprawdza, czy string zawiera jakiekolwiek z podanych słów.
        public static string ToCamelCase(this string str)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            str = str.Trim();
            return char.ToLowerInvariant(str[0]) + str.Substring(1);
        }
        public static bool IsNumeric(this string str)
        {
            return double.TryParse(str, out _);
        }
        public static string Reverse(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            char[] array = str.ToCharArray();
            Array.Reverse(array);
            return new string(array);
        }
        public static string RemoveWhitespace(this string str)
        {
            return string.Join("", str.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
        }
        public static bool ContainsAny(this string str, params string[] words)
        {
            if (string.IsNullOrEmpty(str) || words == null || words.Length == 0)
            {
                return false;
            }

            foreach (var word in words)
            {
                if (str.Contains(word, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }

}
