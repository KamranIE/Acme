using Acme.UI.Helper.Extensions;
using System.Linq;
using System.Web.Routing;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace Acme.UI.Infrastructure.Routing.VirtualRouteHandlers
{
    public class LoginVirtualPathHandler : UmbracoVirtualNodeRouteHandler
    {
        protected override IPublishedContent FindContent(RequestContext requestContext, UmbracoContext umbracoContext)
        {
            Umbraco.Web.PublishedModels.ContentPage loginPage = null;
            var home = umbracoContext.Content.GetAtRoot().FirstOrDefault(x => x.Name == "Home");
            if (home != null)
            {
                loginPage = home.Children<Umbraco.Web.PublishedModels.ContentPage>().FirstOrDefault(x => x.Name == "Login");
            }
            return loginPage;
        }
    }
}