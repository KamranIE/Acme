using System;
using System.Linq.Expressions;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.ModelsBuilder.Umbraco;

namespace Acme.UI.Helper.Extensions
{
    public static class PublishedContentModelExtension
    {
        public static string GetAlias<T1, TValue>(this T1 t1, IPublishedContentType t2, Expression<Func<T1, TValue>> selector) where T1 : PublishedContentModel
        {
            return PublishedModelUtility.GetModelPropertyType(t2, selector).Alias;
        }
    }
}