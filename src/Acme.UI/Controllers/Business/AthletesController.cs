using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Examine;
using Umbraco.Examine;

namespace Acme.UI.Controllers.Business
{
    public class AthletesController : SurfaceController
    {
        public ActionResult GetAthlete(Umbraco.Web.PublishedModels.Athlete athlete)
        {
            return View("~/Views/Athletes/Athlete.cshtml", athlete);
        }


        public ActionResult AthleteDetails(int athleteId)
        {
            var athlete = Umbraco.Content(athleteId);
            return View("~/Views/Athletes/Athlete.cshtml", athlete);
        }

        // GET: Athletes
        public ActionResult List(int? maxAthletesToDisplay)
        {
            var logginPhysioNodeId = GetLoggedInPhysioNodeId();
            var athletes = new List<Umbraco.Web.PublishedModels.Athlete>();
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
                                var node = Umbraco.Content(item.Id) as Umbraco.Web.PublishedModels.Athlete;

                                athletes.Add(node);
                            }
                        }
                    }
                }
            }

            return View(maxAthletesToDisplay.HasValue ? athletes.Take(maxAthletesToDisplay.Value).ToList<Umbraco.Web.PublishedModels.Athlete>() : athletes);
        }

        private int? GetLoggedInPhysioNodeId()
        {
            var member = Members.GetCurrentMember();

            var value = member.Properties.FirstOrDefault(x => x.Alias == "memberDataFolder");

            if (value != null)
            {
                return value.Value<Umbraco.Core.Models.PublishedContent.IPublishedContent>().Id;
            }

            return null;
        }
    }
}