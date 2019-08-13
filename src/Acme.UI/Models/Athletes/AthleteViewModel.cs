using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.PublishedModels;

namespace Acme.UI.Models.Athletes
{
    public class AthleteViewModel : Athlete
    {
       public AthleteViewModel(IPublishedContent content) : base(content)
       {
       }

        public string Salutation
        {
            get
            {
                if (Gender == "Male")
                {
                    return "He";
                }
                else if (Gender == "Female")
                {
                    return "She";
                }

                return Name;
            }
        }
    }
}