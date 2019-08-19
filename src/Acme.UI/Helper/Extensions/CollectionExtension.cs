﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Web.Models;

namespace Acme.UI.Helper.Extensions
{
    public static class CollectionExtension
    {
        public static bool HasValues<T>(this IEnumerable<T> obj)
        {
            return obj != null && obj.Any();
        }

        public static void AddIfNotNull<T>(this ICollection<T> obj, T item)
        {
            if (item != null)
            {
                obj.Add(item);
            }
        }
    }
}