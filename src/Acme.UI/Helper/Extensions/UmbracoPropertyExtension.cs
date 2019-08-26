using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Web.Models;

namespace Acme.UI.Helper.Extensions
{
    public static class UmbracoPropertyExtension
    {
        public static UmbracoProperty GetValue(this List<UmbracoProperty> obj, string alias)
        {
            if (obj.HasValues() && !alias.IsNull())
            {
                return obj.FirstOrDefault(x => alias.Equals(x.Alias));
            }
            return null;
        }

        public static void SetValue(this List<UmbracoProperty> obj, string alias, string value)
        {
            if (obj != null)
            {
                var prop = obj.GetValue(alias);
                if (prop != null)
                {
                    obj.Remove(prop);
                }

                obj.Add(new UmbracoProperty { Alias = alias, Value = value });
            }
        }

        public static bool HasValues(this Umbraco.Core.Models.Property property)
        {
            return property != null && property.Values.HasValues();
        }
    }
}