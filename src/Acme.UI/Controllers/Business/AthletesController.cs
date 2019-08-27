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

            if (athleteName.IsNullOrWhiteSpaceExt())
            {
                return GetAtheleteById(null);
            }

            var logginPhysioNodeId = GetLoggedInPhysioNodeId();
            var athletesForLoggedInUser = GetAthletesForLoggedInUser(logginPhysioNodeId);

            if (athletesForLoggedInUser.HasValues())
            {
                var matchedAthlete = athletesForLoggedInUser.FirstOrDefault(athlete => 
                                                {
                                                    var content = Umbraco.Content(athlete.Id);
                                                    if (content == null)
                                                    {
                                                        return false;
                                                    }
                                                    return athleteName.Equals(content.Name, System.StringComparison.InvariantCultureIgnoreCase);
                                                });

                return matchedAthlete != null ? GetAtheleteById(int.Parse(matchedAthlete.Id)) : GetAtheleteById(null);
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

            if (!athletesForLoggedInUser.HasValues())
            {
                return View("~/Views/BusinessPages/Athletes/List.cshtml", physioAthletesViewModel);
            }

            foreach (var item in athletesForLoggedInUser)
            {
                if (item.Id != null)
                {
                    physioAthletesViewModel.AddAthlete(new AthleteViewModel(Umbraco.Content(item.Id)));
                }
            }

            return View("~/Views/BusinessPages/Athletes/List.cshtml", physioAthletesViewModel);
        }

        private string GetAthleteNameFromUrl(string url)
        {
            Regex regex = new Regex("(?<home>(?i)dashboard)/(?<section>(?i)athlete)/(?<athleteName>[a-zA-Z0-9-]+)");
            // (?i) is used for case insensitive match in each group except athleteName where it is implicit
            // ?<xyz> is the group identifier(or name) which is given to each group in the regex for easy 
            // access to contents matched to a group

            var matches = regex.Matches(url); 

            if (matches == null || matches.Count <= 0)
            {
                return null;
            }

            var groups = (matches[0] as Match)?.Groups;

            if (groups != null)
            {
                return groups["athleteName"]?.Value.Replace("-", " "); // replace hyphens with space for name to name comparison
            }

            return null;
        }

        private ISearchResults GetAthletesForLoggedInUser(int? logginPhysioNodeId)
        {
            if (logginPhysioNodeId == null)
            {
                return null;
            }

            if (ExamineManager.Instance.TryGetIndex("ExternalIndex", out var index))
            {
                var searcher = index.GetSearcher();
                return searcher.CreateQuery("content").ParentId(logginPhysioNodeId.Value).Execute();
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