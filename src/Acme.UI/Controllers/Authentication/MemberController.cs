using Acme.UI.Models.Authentication;
using Umbraco.Web.Mvc;
using System.Web.Mvc;
using System.Web.Security;
using System.Linq;
using Umbraco.Web.PublishedModels;
using Umbraco.Web.Models;
using Umbraco.Core.Services;

namespace Acme.UI.Controllers.Authentication
{
    public class MemberController : SurfaceController
    {
        private IMemberService _memberService;

        public MemberController(IMemberService service)
        {
            _memberService = service;
        }

        public ActionResult RenderLogin(string returnUrl)
        {
            var model = new Models.Authentication.LoginModel
            {
                ReturnUrl = returnUrl
            };

            return PartialView("~/Views/BusinessPages/Authentication/_Login.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitLogin(Models.Authentication.LoginModel model, string returnUrl)
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

        public ActionResult RenderRegistration()
        {
            var register= this.Umbraco.AssignedContentItem as Register;

            var model = new MemberViewModel {
                ConfirmPasswordTitle = register.ConfirmPasswordTitle,
                DisplayNameTitle = register.DisplayNameTitle,
                EmailTitle = register.EmailTitle,
                PasswordTitle = register.PasswordTitle,
                UserNameTitle = register.UserNameTitle,
                AddressTitle = register.AddressTitle,
                PhoneTitle = register.PhoneTitle
            };

            return PartialView("~/Views/BusinessPages/Authentication/_Registration.cshtml", model);
        }

        public ActionResult SubmitRegistration(MemberViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.IsApproved = false;
                var user = Members.RegisterMember(model.RegisterModel, out var status, false);

                if (!HasRegisterMemberErrors(status))
                {
                    _memberService.AssignRole(model.UserName, "Physiotherapist");

                    return Redirect("/login/");
                }
            }
            return CurrentUmbracoPage();

        }

        private bool HasRegisterMemberErrors(MembershipCreateStatus status)
        {
            switch (status)
            {
                case MembershipCreateStatus.DuplicateEmail:
                    ModelState.AddModelError("DuplicateEmail", "Email is already registered with someother user");
                    return true;
                case MembershipCreateStatus.DuplicateUserName:
                    ModelState.AddModelError("DuplicateUserName", "User name is already registered");
                    return true;
                case MembershipCreateStatus.InvalidEmail:
                    ModelState.AddModelError("InvalidEmail", "Email is invalid");
                    return true;

                case MembershipCreateStatus.InvalidPassword:
                    ModelState.AddModelError("InvalidPassword", "Password is invalid");
                    return true;

                case MembershipCreateStatus.UserRejected:
                    ModelState.AddModelError("UserRejected", "User is rejected");
                    return true;
            }
            return false;
        }
    }
}