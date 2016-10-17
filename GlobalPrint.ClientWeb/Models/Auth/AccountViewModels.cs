using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Resources.ClientWeb.Account;
using System.ComponentModel;

namespace GlobalPrint.ClientWeb
{
    public class LoginViewModel
    {
        [Required(ErrorMessageResourceType = typeof(LoginViewResource), ErrorMessageResourceName = "EmailFieldRequiredError", ErrorMessage = null)]
        [EmailAddress(ErrorMessageResourceType = typeof(LoginViewResource), ErrorMessageResourceName = "EmailFieldTypeError", ErrorMessage = null)]
        [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof(LoginViewResource), ErrorMessageResourceName = "EmailFieldTypeError", ErrorMessage = null)]
        [Display(ResourceType = typeof(LoginViewResource), Name = "EmailFieldLabel", Prompt = "EmailFieldLabel")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(LoginViewResource), ErrorMessageResourceName = "PasswordFieldRequiredError", ErrorMessage = null)]
        [DataType(DataType.Password, ErrorMessageResourceType = typeof(LoginViewResource), ErrorMessageResourceName = "PasswordFieldTypeError", ErrorMessage = null)]
        [StringLength(20, MinimumLength = 6, ErrorMessageResourceType = typeof(LoginViewResource), ErrorMessageResourceName = "PasswordFieldMinLengthError", ErrorMessage = null)]
        [Display(ResourceType = typeof(LoginViewResource), Name = "PasswordFieldLabel", Prompt = "PasswordFieldLabel")]
        public string Password { get; set; }
        
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessageResourceType = typeof(RegisterViewResource), ErrorMessageResourceName = "EmailFieldRequiredError")]
        [EmailAddress(ErrorMessageResourceType = typeof(RegisterViewResource), ErrorMessageResourceName = "EmailFieldTypeError")]
        [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof(RegisterViewResource), ErrorMessageResourceName = "EmailFieldTypeError")]
        [Display(ResourceType = typeof(RegisterViewResource), Name = "EmailFieldLabel", Prompt = "EmailFieldLabel")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(RegisterViewResource), ErrorMessageResourceName = "PasswordFieldRequiredError")]
        [DataType(DataType.Password, ErrorMessageResourceType = typeof(RegisterViewResource), ErrorMessageResourceName = "PasswordFieldTypeError")]
        [StringLength(20, MinimumLength = 6, ErrorMessageResourceType = typeof(RegisterViewResource), ErrorMessageResourceName = "PasswordFieldMinLengthError")]
        [Display(ResourceType = typeof(RegisterViewResource), Name = "PasswordFieldLabel", Prompt = "PasswordFieldLabel")]
        public string Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(RegisterViewResource), ErrorMessageResourceName = "ConfirmPasswordFieldRequiredError")]
        [DataType(DataType.Password, ErrorMessageResourceType = typeof(RegisterViewResource), ErrorMessageResourceName = "ConfirmPasswordFieldTypeError")]
        [Display(ResourceType = typeof(RegisterViewResource), Name = "ConfirmPasswordFieldLabel", Prompt = "ConfirmPasswordFieldLabel")]
        [Compare("Password", ErrorMessageResourceType = typeof(RegisterViewResource), ErrorMessageResourceName = "ConfirmationNotEqualToPassword")]
        public string ConfirmPassword { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(RegisterViewResource), ErrorMessageResourceName = "ConfirmOfferRequiredError")]
        [Range(typeof(bool), "true", "true", ErrorMessageResourceType = typeof(RegisterViewResource), ErrorMessageResourceName = "ConfirmOfferRequiredError")]
        public bool IsAgreeWithOffer { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessageResourceType = typeof(ResetPasswordViewResource), ErrorMessageResourceName = "EmailFieldRequiredError")]
        [EmailAddress(ErrorMessageResourceType = typeof(ResetPasswordViewResource), ErrorMessageResourceName = "EmailFieldTypeError")]
        [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof(ResetPasswordViewResource), ErrorMessageResourceName = "EmailFieldTypeError")]
        [Display(ResourceType = typeof(ResetPasswordViewResource), Name = "EmailFieldLabel", Prompt = "EmailFieldLabel")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(ResetPasswordViewResource), ErrorMessageResourceName = "PasswordFieldRequiredError")]
        [DataType(DataType.Password, ErrorMessageResourceType = typeof(ResetPasswordViewResource), ErrorMessageResourceName = "PasswordFieldTypeError")]
        [StringLength(20, MinimumLength = 6, ErrorMessageResourceType = typeof(ResetPasswordViewResource), ErrorMessageResourceName = "PasswordFieldMinLengthError")]
        [Display(ResourceType = typeof(ResetPasswordViewResource), Name = "PasswordFieldLabel", Prompt = "PasswordFieldLabel")]
        public string Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(ResetPasswordViewResource), ErrorMessageResourceName = "ConfirmPasswordFieldRequiredError")]
        [DataType(DataType.Password, ErrorMessageResourceType = typeof(ResetPasswordViewResource), ErrorMessageResourceName = "ConfirmPasswordFieldTypeError")]
        [Display(ResourceType = typeof(ResetPasswordViewResource), Name = "ConfirmPasswordFieldLabel", Prompt = "ConfirmPasswordFieldLabel")]
        [Compare("Password", ErrorMessageResourceType = typeof(ResetPasswordViewResource), ErrorMessageResourceName = "ConfirmationNotEqualToPassword")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessageResourceType = typeof(ForgotPasswordViewResource), ErrorMessageResourceName = "EmailFieldRequiredError")]
        [EmailAddress(ErrorMessageResourceType = typeof(ForgotPasswordViewResource), ErrorMessageResourceName = "EmailFieldTypeError")]
        [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof(ForgotPasswordViewResource), ErrorMessageResourceName = "EmailFieldTypeError")]
        [Display(ResourceType = typeof(ForgotPasswordViewResource), Name = "EmailFieldLabel", Prompt = "EmailFieldLabel")]
        public string Email { get; set; }
    }

    [Obsolete("Пока что убрали регистрацию и логин через телефон")]
    public class VerifyPhoneNumberViewModel
    {
        [Required(ErrorMessage = "Данное поле обязательно для заполнения.")]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Данное поле обязательно для заполнения.")]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }
}