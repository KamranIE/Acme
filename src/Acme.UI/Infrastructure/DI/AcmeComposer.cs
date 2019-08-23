using Acme.UI.Helper.Services;
using Acme.UI.Infrastructure.Handlers;
using Acme.UI.Infrastructure.Routing;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Web;
using Umbraco.Web.Routing;

namespace Acme.UI.Infrastructure.DI
{
    public class AcmeComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            RegisterComponents(composition);
            RegisterServices(composition);
            RegisterContentFinders(composition);
        }

        private void RegisterComponents(Composition composition)
        {
            composition.Components().Append<MemberSaveHandler>();
                                    //.Append<AcmeRouter>();
        }

        private void RegisterServices(Composition composition)
        {
            composition.Register<IAcmeService<string, string>, CustomService>(Lifetime.Request);
            composition.Register<CustomService2>(Lifetime.Transient);
        }

        private void RegisterContentFinders(Composition composition)
        {
            composition.ContentFinders().InsertBefore<ContentFinderByUrl, AcmeContentFinder>();
        }

    }
}