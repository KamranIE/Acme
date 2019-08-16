using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Umbraco.Web.Models;

namespace Acme.UI.Models.Authentication
{
    public class MemberViewModel : IValidatableObject
    {
        private const string umbracoIsApprovedPropAlias = "umbracoMemberApproved";
        private const string addressAlias = "address";
        private const string phoneAlias = "phone";


        public MemberViewModel()
        {
            RegisterModel = RegisterModel.CreateModel();
            RegisterModel.UsernameIsEmail = false;
            RegisterModel.LoginOnSuccess = false;
        }

        public RegisterModel RegisterModel { get; }

        public string DisplayName
        {
            get
            {
                return RegisterModel.Name;
            }
            set
            {
                RegisterModel.Name = value;
            }
        }

        public string UserName
        {
            get
            {
                return RegisterModel.Username;
            }
            set
            {
                RegisterModel.Username = value;
            }
        }

        public string Email
        {
            get
            {
                return RegisterModel.Email;
            }
            set
            {
                RegisterModel.Email = value;
            }
        }

        public string Password
        {
            get
            {
                return RegisterModel.Password;
            }
            set
            {
                RegisterModel.Password = value;
            }
        }

        public string Address
        {
            get
            {
                var found = RegisterModel.MemberProperties.FirstOrDefault(e => e.Alias == addressAlias);
                if (found != null)
                {
                    return found.Value;
                }
                return null; 
            }

            set
            {
                var found = RegisterModel.MemberProperties.FirstOrDefault(e => e.Alias == addressAlias);
                if (found != null)
                {
                    RegisterModel.MemberProperties.Remove(found);
                }
                RegisterModel.MemberProperties.Add(new UmbracoProperty { Alias = addressAlias, Value = value});
            }
        }

        public string Phone
        {
            get
            {
                var found = RegisterModel.MemberProperties.FirstOrDefault(e => e.Alias == phoneAlias);
                if (found != null)
                {
                    return found.Value;
                }
                return null;
            }

            set
            {
                var found = RegisterModel.MemberProperties.FirstOrDefault(e => e.Alias == phoneAlias);
                if (found != null)
                {
                    RegisterModel.MemberProperties.Remove(found);
                }
                RegisterModel.MemberProperties.Add(new UmbracoProperty { Alias = phoneAlias, Value = value });
            }
        }

        public bool IsApproved
        {
            get
            {
                var found = RegisterModel.MemberProperties.FirstOrDefault(e => e.Alias == umbracoIsApprovedPropAlias);
                if (found != null)
                {
                    return found.Value != "0";
                }
                return true; // it is true in all other cases while registering the user
            }

            set
            {
                var found = RegisterModel.MemberProperties.FirstOrDefault(e => e.Alias == umbracoIsApprovedPropAlias);
                if (found != null)
                {
                    RegisterModel.MemberProperties.Remove(found);
                }
                RegisterModel.MemberProperties.Add(new UmbracoProperty { Alias = umbracoIsApprovedPropAlias, Value = (value ? "1" : "0") });
            }
        }

        public string ConfirmPassword { get; set; }

        public string DisplayNameTitle { get; set; }

        public string UserNameTitle { get; set; }

        public string EmailTitle { get; set; }

        public string AddressTitle { get; set; }

        public string PhoneTitle { get; set; }

        public string PasswordTitle { get; set; }

        public string ConfirmPasswordTitle { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(DisplayName))
            {
                yield return new ValidationResult(DisplayNameTitle + " is mandatory");
            }

            if (string.IsNullOrWhiteSpace(UserName))
            {
                yield return new ValidationResult(UserNameTitle + " is mandatory");
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                yield return new ValidationResult(EmailTitle + " is mandatory");
            }

            if (string.IsNullOrWhiteSpace(Address))
            {
                yield return new ValidationResult(AddressTitle + " is mandatory");
            }

            if (string.IsNullOrWhiteSpace(Phone))
            {
                yield return new ValidationResult(PhoneTitle + " is mandatory");
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                yield return new ValidationResult(PasswordTitle + " is mandatory");
            }

            if (string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                yield return new ValidationResult(ConfirmPasswordTitle + " is mandatory");
            }

            if (string.Compare(Password, ConfirmPassword) != 0)
            {
                yield return new ValidationResult(PasswordTitle + " and "+ ConfirmPasswordTitle + " do not match.");
            }
        }
    }
}