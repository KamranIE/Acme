using Umbraco.Core.Composing;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;

namespace Acme.UI.Infrastructure.Handlers
{
    [RuntimeLevel(MinLevel = Umbraco.Core.RuntimeLevel.Run)]
    public class PublishHandlerComposer : ComponentComposer<PublishHandler>
    {
    }

    public class PublishHandler : IComponent
    {
        private ILogger _logger;

        public PublishHandler(ILogger logger)
        {
            _logger = logger;
        }

        public void Initialize()
        {
            // ContentService.Published += this.OnPublish;
        }

        public void Terminate()
        {
            _logger.Info<PublishHandler>("Terminating...");
        }

        private void OnPublish(IContentService sender, ContentPublishedEventArgs e)
        {
            _logger.Info<PublishHandler>("on publish started");
            foreach (var publishedItem in e.PublishedEntities)
            {
                _logger.Info<PublishHandler>(publishedItem.Name + " was published");
            }
            _logger.Info<PublishHandler>("on publish ");
        }
    }
}