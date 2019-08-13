using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.PublishedModels;

namespace Acme.UI.Models.Athletes
{
    public class PhysioAthletesViewModel
    {
        private List<AthleteViewModel> _athletes;

        private PhysioAthletesViewModel(IPublishedContent containerItem, Physiotherapist physio, int maxAthletesToDisplay)
        {
            _athletes = new List<AthleteViewModel>();
            MaxAthletesToDisplay = maxAthletesToDisplay;
            if (containerItem != null)
            {
                SetUpBaseUrl(containerItem.Url);
            }
            SetupPhysioDataData(physio);
        }

        private void SetUpBaseUrl(string url)
        {
            if (url != null)
            {
                BaseUrl = url;
                var lastChar = url.Last();

                if (lastChar != '/' && lastChar != '\\')
                {
                    BaseUrl += "/";
                }

                BaseUrl += "athlete";
            }
        }

        private void SetupPhysioDataData(Physiotherapist physio)
        {
            if (physio != null)
            {
                PhysioName = physio.Physio_name;
            }
        }

        public void AddAthlete(AthleteViewModel athlete)
        {
            _athletes.Add(athlete);
        }

        public IReadOnlyList<AthleteViewModel> AllAthletes
        {
            get {
                return _athletes;
            }        
        }

        public IReadOnlyList<AthleteViewModel> DisplayableAthletes
        {
            get
            {
                return MaxAthletesToDisplay > 0 ? _athletes.Take(MaxAthletesToDisplay).ToList() : _athletes;
            }
        }

        public int MaxAthletesToDisplay { get; private set; }
        public string BaseUrl { get; private set; } = string.Empty;
        public string PhysioName { get; private set; }

        public static PhysioAthletesViewModel Create(Umbraco.Web.UmbracoHelper umbraco, int physioNodeId, int? maxAthletesToDisplay)
        {
            var assignedItem = umbraco.AssignedContentItem;
            var physio = umbraco.Content(physioNodeId) as Physiotherapist;

            return new PhysioAthletesViewModel(assignedItem, physio, maxAthletesToDisplay ?? 0);
        }
    }
}