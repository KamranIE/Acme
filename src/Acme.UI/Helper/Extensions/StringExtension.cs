using System.Text.RegularExpressions;

namespace Acme.UI.Helper.Extensions
{
    public static class StringExtension
    {
        public static bool IsNull(this string obj)
        {
            return obj == null;
        }

        public static bool IsNullOrWhiteSpaceExt(this string obj)
        {
            return obj.IsNull() || string.IsNullOrWhiteSpace(obj);
        }

        public static bool IsObjectAsStringNullOrWhitespace(this object obj)
        {
            if (obj != null)
            {
                var str = obj as string;

                return str.IsNullOrWhiteSpaceExt();
            }

            return true;
        }

        public static bool ContainsUrl(this string str, string path)
        {
            if (str == null || path == null)
            {
                return false;
            }

            var formatedPath = ReplacePathSheshesWithTilda(path).ToLower();
            var formatedStr = ReplacePathSheshesWithTilda(str).ToLower();

            return formatedStr.Contains(formatedPath);
        }


        private static string ReplacePathSheshesWithTilda(string path)
        {
            string replacement = "τ"; // a char difficult  τ = 999
            Regex regEx = new Regex("[\\/]");
            var pathWithTilda = regEx.Replace(path, replacement);

            // also remove starting and ending "slash" (already replaced with special char) presenence for better compare
            return pathWithTilda.TrimStart(replacement[0]).TrimEnd(replacement[0]);
        }

    }


}