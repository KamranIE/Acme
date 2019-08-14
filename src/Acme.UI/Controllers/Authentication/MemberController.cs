using Acme.UI.Models.Authentication;
using Umbraco.Web.Mvc;
using System.Web.Mvc;
using System.Web.Security;

namespace Acme.UI.Controllers.Authentication
{
    public class MemberController : SurfaceController
    {
        public ActionResult RenderLogin(string returnUrl)
        {
            var model = new LoginModel
            {
                ReturnUrl = returnUrl
            };

            return PartialView("~/Views/BusinessPages/Authentication/_Login.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitLogin(LoginModel model, string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = "/dashboard/";
            }

            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.Username, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.Username, false);
                    UrlHelper helper = new UrlHelper(HttpContext.Request.RequestContext);
                    if (helper.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return Redirect("/");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username/password");
                }
            }
            return CurrentUmbracoPage();
        }

        
        public ActionResult RenderLogout(string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = "/";
            }

            return PartialView("~/Views/BusinessPages/Authentication/_Logout.cshtml", returnUrl);
        }

        public ActionResult SubmitLogout(string returnUrl)
        {
            TempData.Clear();
            Session.Clear();
            FormsAuthentication.SignOut();
            return Redirect(returnUrl);
        }
    }
}