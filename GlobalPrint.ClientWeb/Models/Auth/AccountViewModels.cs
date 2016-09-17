using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GlobalPrint.ClientWeb
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Данное поле обязательно для заполнения.")]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Данное поле обязательно для заполнения.")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Минимальная длина пароля - {2} символов.", MinimumLength = 6)]
        public string Password { get; set; }
        
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Данное поле обязательно для заполнения.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Данное поле обязательно для заполнения.")]
        [StringLength(100, ErrorMessage = "Минимальная длина пароля - {2} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Данное поле обязательно для заполнения.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароль и его подтверждение не совпадают.")]
        public string ConfirmPassword { get; set; }
        
        [Required(ErrorMessage = "Вы должны подтвердить свое согласие с условиями оферты.")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Вы должны подтвердить свое согласие с условиями оферты.")]
        public bool IsAgreeWithTerms { get; set; }
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
        [Required(ErrorMessage = "Данное поле обязательно для заполнения.")]
        [EmailAddress]
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