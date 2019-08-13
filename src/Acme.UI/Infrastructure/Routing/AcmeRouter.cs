using Umbraco.Core.Composing;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Strings;


namespace Acme.UI.Infrastructure.Routing
{
    public class AcmeRouterComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.UrlSegmentProviders().InsertBefore<DefaultUrlSegmentProvider, AcmeRouter>();
        }
    }

    public class AcmeRouter : IUrlSegmentProvider
    {
        readonly IUrlSegmentProvider _provider = new DefaultUrlSegmentProvider();

        public string GetUrlSegment(IContentBase content, string culture = null)
        {
            if (content.ContentType.Alias != "athlete") return null;

            var segment = _provider.GetUrlSegment(content);

            var productId = content.GetValue("id");
            return string.Format("{0}-{1}", segment, productId);
        }
    }
}