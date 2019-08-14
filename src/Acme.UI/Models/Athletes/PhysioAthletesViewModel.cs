using Acme.UI.Helper.Extensions;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.PublishedModels;

namespace Acme.UI.Models.Athletes
{
    public class PhysioAthletesViewModel
    {
        private List<AthleteViewModel> _athletes;

        private PhysioAthletesViewModel(IPublishedContent containerItem, string physioName, int maxAthletesToDisplay)
        {
            _athletes = new List<AthleteViewModel>();
            MaxAthletesToDisplay = maxAthletesToDisplay;
            if (containerItem != null)
            {
                BaseUrl = GetBaseUrl(containerItem.Url);
            }
            PhysioName = physioName; 
        }

        private string GetBaseUrl(string url)
        {
            if (!url.IsNull())
            {
                var lastChar = url.Last();

                if (lastChar != '/' && lastChar != '\\')
                {
                    url += "/";
                }

                url += "athlete";
            }
            return url;
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

            return new PhysioAthletesViewModel(assignedItem, physio?.Physio_name, maxAthletesToDisplay ?? 0);
        }
    }
}