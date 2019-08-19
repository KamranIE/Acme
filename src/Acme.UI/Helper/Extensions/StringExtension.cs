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
    }


}