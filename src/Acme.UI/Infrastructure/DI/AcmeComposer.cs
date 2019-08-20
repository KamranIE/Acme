using Acme.UI.Helper.Services;
using Acme.UI.Infrastructure.Handlers;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Acme.UI.Infrastructure.DI
{
    public class AcmeComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            RegisterComponents(composition);
            RegisterServices(composition);
        }

        private void RegisterComponents(Composition composition)
        {
            composition.Components().Append<MemberSaveHandler>();
        }

        private void RegisterServices(Composition composition)
        {
            composition.Register<IAcmeService<string, string>, CustomService>(Lifetime.Request);
            composition.Register<CustomService2>(Lifetime.Transient);
        }

    }
}