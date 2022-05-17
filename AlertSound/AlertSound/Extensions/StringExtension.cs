using System.Linq;

namespace AlertSound.Extensions
{
    public static class StringExtension
    {
        public static string CleanSpace(this string value)
        {
            if (value == null)
                return string.Empty;

            return value.Trim();
        }

        public static string WithOutSpace(this string value)
        {
            if (value == null)
                return string.Empty;

            return value.Trim().Replace(" ", "");
        }

        public static string ToAllFirstLetterInUpper(this string value)
        {
            if (value == null)
                return string.Empty;

            value.CleanSpace();

            var array = value.Split(' ');

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == "" || array[i] == " " || listOfArticles_Prepositions().Contains(array[i])) continue;
                array[i] = array[i].ToFirstLetterUpper();
            }

            return string.Join(" ", array);
        }

        private static string ToFirstLetterUpper(this string str)
        {
            return str?.First().ToString().ToUpper() + str?.Substring(1).ToLower();
        }

        private static string[] listOfArticles_Prepositions()
        {
            return new[]
            {
               /*EN*/ "in","on","to","of","and","or","for","a","an","is",
               /*ES*/"en","de","y","o","por","a","es"
            };
        }
    }
}
