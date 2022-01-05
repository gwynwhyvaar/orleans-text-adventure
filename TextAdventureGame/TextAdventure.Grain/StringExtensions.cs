using System.Text;

namespace TextAdventure.Grain
{
    public static class StringExtensions
    {
        public static string RemoveStopWords(this string s)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string[] stopWards = new string[] { "on", "the", "a" };
            foreach (var word in stopWards)
            {
                stringBuilder.Replace(word, string.Empty);
            }
            return stringBuilder.ToString();
        }
        public static string Rest(this string[] words)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for(int i = 1; i< words.Length; i++)
            {
                stringBuilder.Append($"{words[i]} ");
            }
            return stringBuilder.ToString().Trim().ToLower();
        }
    }
}
