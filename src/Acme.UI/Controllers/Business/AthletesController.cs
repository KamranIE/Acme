using System.Web.Mvc;
using Umbraco.Web.Mvc;
using Umbraco.Examine;
using Acme.UI.Models.Athletes;
using Umbraco.Web.PublishedModels;
using Acme.UI.Helper.Services;
using Acme.UI.Helper.Extensions;

namespace Acme.UI.Controllers.Business
{
    public class AthletesController : SurfaceController
    {
        private ExamineService _examineService;
        
        public AthletesController(ExamineService examineService)
        {
            _examineService = examineService;
        }

        public ActionResult AthleteDetails(int athleteId)
        {
            var athlete = new AthleteViewModel(Umbraco.Content(athleteId));
            return View("~/Views/BusinessPages/Athletes/Athlete.cshtml", athlete);
        }

        public ActionResult List(int? maxAthletesToDisplay)
        {
            var logginPhysioNodeId = GetLoggedInPhysioNodeId();

            var physioAthletesViewModel = PhysioAthletesViewModel.Create(Umbraco, logginPhysioNodeId ?? 0, maxAthletesToDisplay);

            if (logginPhysioNodeId == null)
            {
                return View("~/Views/BusinessPages/Athletes/List.cshtml", physioAthletesViewModel);
            }

            var results = _examineService.Query?.ParentId(logginPhysioNodeId.Value).Execute();

            if (results.HasValues())
            {
                foreach (var item in results)
                {
                    if (!item.Id.IsNull())
                    {
                        physioAthletesViewModel.AddAthlete(new AthleteViewModel(Umbraco.Content(item.Id)));
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