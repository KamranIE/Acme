using Examine;
using Lucene.Net.Search;
using System.Linq;
using System.Web.Mvc;
using Umbraco.Web.Mvc;


namespace Acme.UI.Controllers.Business
{
    public class AthletesController : SurfaceController
    {
        // GET: Athletes
        public ActionResult List(int? maxAthletesToDisplay)
        {
            if (ExamineManager.Instance.TryGetIndex("ExternalIndex", out var index))
            {
                var searcher = index.GetSearcher();
                var results = searcher.CreateQuery().Field("__NodeTypeAlias", "athlete").And().Field("path", "1212".MultipleCharacterWildcard()).Execute();
                if (results.Any())
                {
                    int a = 1;
                }
                else
                {

                }
            }
            return View(maxAthletesToDisplay ?? 0);
        }
    }
}