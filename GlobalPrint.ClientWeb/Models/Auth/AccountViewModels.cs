using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Resources.ClientWeb.Account;

namespace GlobalPrint.ClientWeb
{
    public class LoginViewModel
    {
        [Required(ErrorMessageResourceType = typeof(LoginViewResource), ErrorMessageResourceName = "EmailFieldRequiredError")]
        [EmailAddress(ErrorMessageResourceType = typeof(LoginViewResource), ErrorMessageResourceName = "EmailFieldTypeError")]
        [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof(LoginViewResource), ErrorMessageResourceName = "EmailFieldTypeError")]
        [Display(ResourceType = typeof(LoginViewResource), Name = "EmailFieldLabel", Prompt = "EmailFieldLabel")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(LoginViewResource), ErrorMessageResourceName = "PasswordFieldRequiredError")]
        [DataType(DataType.Password, ErrorMessageResourceType = typeof(LoginViewResource), ErrorMessageResourceName = "PasswordFieldTypeError")]
        [StringLength(20, MinimumLength = 6, ErrorMessageResourceType = typeof(LoginViewResource), ErrorMessageResourceName = "PasswordFieldMinLengthError")]
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
        [Required(ErrorMessage = "Данное поле обязательно для заполнения.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Данное поле обязательно для заполнения.")]
        [StringLength(100, ErrorMessage = "Минимальная длина пароля - {2} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Данное поле обязательно для заполнения.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароль и его подтверждение не совпадают.")]
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