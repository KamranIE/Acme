using Acme.UI.Infrastructure.Routing.VirtualRouteHandlers;
using System.Web.Routing;
using Umbraco.Core.Composing;
using Umbraco.Web;
using Umbraco.Core.Logging;

namespace Acme.UI.Infrastructure.Routing
{
    public class AcmeRouter : IComponent
    {
        private ILogger _logger;

        public AcmeRouter(ILogger logger)
        {
            _logger = logger;
        }

        public void Initialize()
        {
            _logger.Info<AcmeRouter>("-->Initializing custom acme routing4");

            /*RouteTable.Routes.MapRoute("LoginRoute1", "kamran/member/render_login/{returnUrl}",
                                                new { controller = "Member", action = "RenderLogin", returnUrl = ""});*/

            RouteTable.Routes.MapUmbracoRoute("LoginRoute1", "kamran/member/render_login/{returnUrl}",
                                                new { controller = "RenderMvc", action = "Index", returnUrl = "" }, new LoginVirtualPathHandler());
            _logger.Info<AcmeRouter>("<--Initialized custom acme routing4");
        }

        public void Terminate()
        {
            return;
        }
    }
}