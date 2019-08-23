using System.Linq;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using Examine;
using Umbraco.Examine;
using Acme.UI.Models.Athletes;
using Umbraco.Web.PublishedModels;
using Umbraco.Core.Models.PublishedContent;

namespace Acme.UI.Controllers.Business
{
    public class AthletesController : SurfaceController
    {
        public ActionResult AthleteDetails(int athleteId)
        {
            return GetAtheleteById(athleteId);
        }

        public ActionResult AthleteDetails()
        {
            //Write code here to look at 
            //Request.Url, parse it, get the athelete name, lookup id call this thing
            return GetAtheleteById(1);
        }

        public ActionResult GetAtheleteById(int atheleteId) {
            var athlete = new AthleteViewModel(Umbraco.Content(athleteId));
            return View("~/Views/BusinessPages/Athletes/Athlete.cshtml", athlete);
        }

        public ActionResult List(int? maxAthletesToDisplay)
        {
            var logginPhysioNodeId = GetLoggedInPhysioNodeId();
            
            var physioAthletesViewModel = PhysioAthletesViewModel.Create(Umbraco, logginPhysioNodeId ?? 0, maxAthletesToDisplay);
            
            if (logginPhysioNodeId != null)
            {
                if (ExamineManager.Instance.TryGetIndex("ExternalIndex", out var index))
                {
                    var searcher = index.GetSearcher();
                    var results = searcher.CreateQuery("content").ParentId(logginPhysioNodeId.Value).Execute();

                    if (results.Any())
                    {
                        foreach (var item in results)
                        {
                            if (item.Id != null)
                            {
                                physioAthletesViewModel.AddAthlete(new AthleteViewModel(Umbraco.Content(item.Id)));
                            }
                        }
                    }
                }
            }

            return View("~/Views/BusinessPages/Athletes/List.cshtml", physioAthletesViewModel);
        }

        private int? GetLoggedInPhysioNodeId()
        {
            var member = Members.GetCurrentMember() as Member;
            
            return member?.MemberDataFolder?.Id;
        }
    }
}