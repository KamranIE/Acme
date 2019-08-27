using Acme.UI.Helper.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.Routing;

namespace Acme.UI.Infrastructure.Routing
{
    public class AcmeContentFinder : IContentFinder
    {
        private readonly Dictionary<string, RouteParameters> _pathToContentType = new Dictionary<string, RouteParameters>
        {
            // (?i) is used for case insensitive match 
            { "(?i)dashboard/athlete/[a-zA-Z0-9-]+", new RouteParameters(typeof(Umbraco.Web.PublishedModels.Athlete), "Athlete", "//home/contentPage/contentPage")}
            
        };

        public bool TryFindContent(PublishedRequest request)
        {
            var path = request.Uri.GetAbsolutePathDecoded();

            request.PublishedContent = ProcessRequest(path, request.UmbracoContext);

            return request.PublishedContent != null;
        }

        private IPublishedContent ProcessRequest(string path, UmbracoContext umbracoContext)
        {
            foreach (var route in _pathToContentType)
            {
                Regex regex = new Regex(route.Key);
                var matches = regex.Matches(path);

                if (matches == null || matches.Count <= 0)
                {
                    return null;
                }

                var groups = (matches[0] as System.Text.RegularExpressions.Match)?.Groups;

                if (groups == null)
                {
                    return null;
                }

                var publishedContents = umbracoContext.Content.GetByXPath(route.Value.RootNodeName);

                if (publishedContents.HasValues())
                {
                    return publishedContents.FirstOrDefault(x => string.Compare(x.Name, route.Value.SearchNodeByName, true) == 0);
                }
            }

            return null;
        }
    }
}