using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Acme.UI.Helper.Extensions
{
    public static class CollectionExtension
    {
        public static bool HasValues<T>(this IEnumerable<T> obj)
        {
            return obj != null && obj.Any();
        }
    }
}