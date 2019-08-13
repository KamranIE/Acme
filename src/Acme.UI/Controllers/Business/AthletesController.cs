using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Examine;
using Umbraco.Examine;
using Umbraco.Core.Models.PublishedContent;
using Acme.UI.Models.Athletes;

namespace Acme.UI.Controllers.Business
{
    public class AthletesController : SurfaceController
    {
        public ActionResult AthleteDetails(int athleteId)
        {
            var athlete = new AthleteDetails(Umbraco.Content(athleteId));
            return View("~/Views/Athletes/Athlete.cshtml", athlete);
        }

        // GET: Athletes
        public ActionResult List(int? maxAthletesToDisplay)
        {
            var logginPhysioNodeId = GetLoggedInPhysioNodeId();
            var athletes = new List<AthleteDetails>();

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
                                var node = new AthleteDetails(Umbraco.Content(item.Id));

                                athletes.Add(node);
                            }
                        }
                    }
                }
            }

            return View(maxAthletesToDisplay.HasValue ? athletes.Take(maxAthletesToDisplay.Value).ToList() : athletes);
        }

        private int? GetLoggedInPhysioNodeId()
        {
            var member = Members.GetCurrentMember();

            var value = member.Properties.FirstOrDefault(x => x.Alias == "memberDataFolder");

            if (value != null)
            {
                return value.Value<IPublishedContent>().Id;
            }

            return null;
        }
    }
}