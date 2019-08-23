using System.Linq;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using Examine;
using Umbraco.Examine;
using Acme.UI.Models.Athletes;
using Umbraco.Web.PublishedModels;
using System.Text.RegularExpressions;
using Acme.UI.Helper.Extensions;

namespace Acme.UI.Controllers.Business
{
    public class AthletesController : SurfaceController
    {
        public ActionResult AthleteDetails(int? athleteId)
        {
            if (athleteId.HasValue)
            {
                return GetAtheleteById(athleteId.Value);
            }

            var athleteName = GetAthleteNameFromUrl(this.Request.Url.OriginalString);

            if (!athleteName.IsNullOrWhiteSpaceExt())
            {
                var logginPhysioNodeId = GetLoggedInPhysioNodeId();
                var athletesForLoggedInUser = GetAthletesForLoggedInUser(logginPhysioNodeId);

                if (athletesForLoggedInUser.HasValues())
                {
                    var matchedAthlete = athletesForLoggedInUser.FirstOrDefault(athlete => athleteName.Equals(Umbraco.Content(athlete.Id).Name, System.StringComparison.InvariantCultureIgnoreCase));

                    return matchedAthlete != null ? GetAtheleteById(int.Parse(matchedAthlete.Id)) : GetAtheleteById(null);
                }
            }

            return GetAtheleteById(null);
        }

        public ActionResult GetAtheleteById(int? athleteId)
        {
            var content = athleteId.HasValue ? Umbraco.Content(athleteId) : null;
            var athlete = content != null ? new AthleteViewModel(content) : null;
            return View("~/Views/BusinessPages/Athletes/Athlete.cshtml", athlete);
        }

        public ActionResult List(int? maxAthletesToDisplay)
        {
            var logginPhysioNodeId = GetLoggedInPhysioNodeId();
            var athletesForLoggedInUser = GetAthletesForLoggedInUser(logginPhysioNodeId);
            var physioAthletesViewModel = PhysioAthletesViewModel.Create(Umbraco, logginPhysioNodeId ?? 0, maxAthletesToDisplay);
            
            if (athletesForLoggedInUser != null)
            {
                if (athletesForLoggedInUser.Any())
                {
                    foreach (var item in athletesForLoggedInUser)
                    {
                        if (item.Id != null)
                        {
                            physioAthletesViewModel.AddAthlete(new AthleteViewModel(Umbraco.Content(item.Id)));
                        }
                    }
                }
            }

            return View("~/Views/BusinessPages/Athletes/List.cshtml", physioAthletesViewModel);
        }

        private string GetAthleteNameFromUrl(string url)
        {
            Regex regex = new Regex("(?<home>dashboard)/(?<section>athlete)/(?<athleteName>[a-zA-Z0-9-]+)");
            var matches = regex.Matches(url);

            if (matches != null && matches.Count > 0)
            {
                var groups = (matches[0] as System.Text.RegularExpressions.Match)?.Groups;

                if (groups != null)
                {
                    return groups["athleteName"]?.Value;
                }
            }

            return null;
        }

        private ISearchResults GetAthletesForLoggedInUser(int? logginPhysioNodeId)
        {

            if (logginPhysioNodeId != null)
            {
                if (ExamineManager.Instance.TryGetIndex("ExternalIndex", out var index))
                {
                    var searcher = index.GetSearcher();
                    return searcher.CreateQuery("content").ParentId(logginPhysioNodeId.Value).Execute();
                }
            }

            return null;
        }

        private int? GetLoggedInPhysioNodeId()
        {
            var member = Members.GetCurrentMember() as Member;
            
            return member?.MemberDataFolder?.Id;
        }
    }
}