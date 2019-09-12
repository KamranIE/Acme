using Examine;
using Examine.Search;

namespace Acme.UI.Helper.Services
{
    public class ExamineService
    {
        public IQuery Query
        {
            get
            {
                if (ExamineManager.Instance.TryGetIndex("ExternalIndex", out var index))
                {
                    return index.GetSearcher().CreateQuery("content");
                }

                return null;
            }
        }
    }
}