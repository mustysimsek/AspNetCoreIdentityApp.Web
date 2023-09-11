using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Core.ViewModels
{
	public class SignUpViewModel
	{
		public SignUpViewModel()
		{

		}
		public SignUpViewModel(string userName, string email, string phone, string password, string passwordConfirm)
		{
			Username = userName;
			Email = email;
			Phone = phone;
			Password = password;
			PasswordConfirm = passwordConfirm;
		}
		[Required(ErrorMessage = "Kullanıcı ad alanı boş bırakılamaz")]
		[Display(Name = "Kullanıcı Adı :")]
		public string Username { get; set; }

		[EmailAddress(ErrorMessage ="Email formatı yanlış")]
        [Required(ErrorMessage = "Email alanı boş bırakılamaz")]
        [Display(Name = "Email :")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telefon alanı boş bırakılamaz")]
        [Display(Name = "Telefon :")]
        public string Phone { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz")]
        [Display(Name = "Şifre :")]
        [MinLength(6, ErrorMessage = "Şifreniz en az 6 karakter olabilir")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password),ErrorMessage ="Şifre aynı değil")]
        [Required(ErrorMessage = "Şifre tekrar alanı boş bırakılamaz")]
        [Display(Name = "Şifre Tekrar :")]
        [MinLength(6, ErrorMessage = "Şifreniz en az 6 karakter olabilir")]
        public string PasswordConfirm { get; set; }
    }
}

