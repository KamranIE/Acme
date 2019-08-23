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
            { "(?<home>dashboard)/(?<section>athlete)/(?<athleteName>[a-zA-Z0-9-]+)", new RouteParameters(typeof(Umbraco.Web.PublishedModels.Athlete), "athleteName", "//data/physiotherapists/physiotherapist")}
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

                if (matches != null && matches.Count > 0)
                {
                    var groups = (matches[0] as System.Text.RegularExpressions.Match)?.Groups;
                    if (groups != null)
                    {
                        var parameterValue = groups[route.Value.SearchNodeByName];
                        var searchValue = parameterValue?.Value ?? route.Value.SearchNodeByName;

                        var home = umbracoContext.Content.GetByXPath(route.Value.RootNodeName);

                        if (home.HasValues())
                        {
                            var parent = home.FirstOrDefault(x => string.Compare(x.Name, HttpContext.Current.User.Identity.Name, true) == 0);

                            return parent?.Children?.FirstOrDefault(child => child.GetType() == route.Value.DataSourceType && string.Compare(child.Name, searchValue, true) == 0);
                        }
                    }
                }
            }

            return null;
        }
    }
}