using Acme.UI.Helper.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Umbraco.Web.Models;
using Umbraco.Web.PublishedModels;

namespace Acme.UI.Models.Authentication
{
    public class MemberViewModel : IValidatableObject
    {
        private Member _member;

        public MemberViewModel()
        {
            _member = new Member(null);
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
                var found = RegisterModel.MemberProperties.GetValue(GetAlias(x => x.Address));
                return found?.Value;
            }

            set
            {
                RegisterModel.MemberProperties.SetValue(GetAlias(x => x.Address), value);
            }
        }

        public string Phone
        {
            get
            {
                var found = RegisterModel.MemberProperties.GetValue(GetAlias(x => x.Phone));
                return found?.Value;
            }

            set
            {
                RegisterModel.MemberProperties.SetValue(GetAlias(x => x.Phone), value);
            }
        }

        public bool IsApproved
        {
            get
            {
                var found = RegisterModel.MemberProperties.GetValue(GetAlias(x => x.UmbracoMemberApproved));
                return found != null ? found.Value != "0" : true;
            }

            set
            {
                RegisterModel.MemberProperties.SetValue(GetAlias(x => x.UmbracoMemberApproved), (value ? "1" : "0"));
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
            List<ValidationResult> result = new List<ValidationResult>();

            result.AddRange(ValidateMandatoryFields());

            if (string.Compare(Password, ConfirmPassword) != 0)
            {
                result.Add(new ValidationResult(PasswordTitle + " and " + ConfirmPasswordTitle + " do not match."));
            }

            return result;
        }

        private IEnumerable<ValidationResult> ValidateMandatoryFields()
        {
            List<ValidationResult> result = new List<ValidationResult>();
            foreach (var field in new Tuple<string, string>[] { new Tuple<string, string>(DisplayNameTitle, DisplayName),
                                                                new Tuple<string, string>(UserNameTitle, UserName),
                                                                new Tuple<string, string>(EmailTitle, Email),
                                                                new Tuple<string, string>(AddressTitle, Address),
                                                                new Tuple<string, string>(PhoneTitle, Phone),
                                                                new Tuple<string, string>(PasswordTitle, Password),
                                                                new Tuple<string, string>(ConfirmPasswordTitle, ConfirmPassword),})
            {
                result.AddIfNotNull(CheckProperty(field.Item1, field.Item2));
            }

            return result;
        }

        private ValidationResult CheckProperty(string propertyTitle, string propertyValue)
        {
            if (propertyValue.IsNullOrWhiteSpaceExt())
            { 
                return new ValidationResult($"{propertyTitle} is mandatory");
            }

            return null;
        }

        private string GetAlias<TValue>(Expression<Func<Member, TValue>> selector)
        {
            return _member.GetAlias(Member.GetModelContentType(), selector);
        }
    }
}