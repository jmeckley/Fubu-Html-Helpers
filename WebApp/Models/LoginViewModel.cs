using System;
using FluentValidation;
using FluentValidation.Attributes;

namespace WebApp.Models
{
    [Validator(typeof(LoginViewModelValidator))]
    public class LoginViewModel
    {
        public Guid Code { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
        
        public string ConfirmPassword { get; set; }

        public Answser? AreYouIn { get; set; }

        public bool YesOrNo { get; set; }
    }

    public enum Answser
    {
        Yes,
        Maybe,
        No
    }

    public class LoginViewModelValidator
        : AbstractValidator<LoginViewModel>
    {
        public LoginViewModelValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().Matches("(?!^[0-9]*$)(?!^[a-zA-Z]*$)^([a-zA-Z0-9]{8,15})$");
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password);
            RuleFor(x => x.AreYouIn).NotEmpty().IsInEnum();
            RuleFor(x => x.YesOrNo).Equal(true);
        }
    }
}