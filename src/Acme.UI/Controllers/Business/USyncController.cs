using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Umbraco.Web.WebApi;
using uSync8.BackOffice;
using Acme.UI.Helper.Extensions;

namespace Acme.UI.Controllers.Business
{
    
    public class USyncController : UmbracoApiController
    {
        private uSyncService _uSyncService;
        private IEnumerable<object> Environment;

        public USyncController(uSyncService uSyncService)
        {
            _uSyncService = uSyncService;
        }
    

        [HttpGet]
        public string GetAutoImportContent(string path)
        {
            var result = _uSyncService.Import(path, true);

            var res = new List<string>(GetSuccessMessages(result));
            res.AddRange(GetFailureMessages(result));
            var msg = string.Join(System.Environment.NewLine, res);

            return "Completed Import. \n" + msg; // we need to further check action semantics in terms of import success or failure. so far the
        }


        private IEnumerable<string> GetSuccessMessages(IEnumerable<uSyncAction> actions)
        {
            if (!actions.HasValues())
            {
                return null;
            }

            return actions.Where(act => act.Success).Select(x => "[Successfully Processed: " +  x.FileName + "]") .ToList();
        }

        private IEnumerable<string> GetFailureMessages(IEnumerable<uSyncAction> actions)
        {
            if (!actions.HasValues())
            {
                return null;
            }

            return actions.Where(act => !act.Success).Select(x => "[Failed: " + x.FileName + " - " + x.Message + "]").ToList();
        }
    }
}