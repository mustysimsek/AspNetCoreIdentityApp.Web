using System;
using System.ComponentModel.DataAnnotations;
using AspNetCoreIdentityApp.Core.Models;
using Microsoft.AspNetCore.Http;

namespace AspNetCoreIdentityApp.Core.ViewModels
{
	public class UserEditViewModel
	{
        [Required(ErrorMessage = "Kullanıcı ad alanı boş bırakılamaz")]
        [Display(Name = "Kullanıcı Adı :")]
        public string Username { get; set; }

        [EmailAddress(ErrorMessage = "Email formatı yanlış")]
        [Required(ErrorMessage = "Email alanı boş bırakılamaz")]
        [Display(Name = "Email :")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telefon alanı boş bırakılamaz")]
        [Display(Name = "Telefon :")]
        public string Phone { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Doğum Tarihi :")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Şehir :")]
        public string? City { get; set; }

        [Display(Name = "Profil Resmi :")]
        public IFormFile? Picture { get; set; }

        [Display(Name = "Cinsiyet :")]
        public Gender? Gender { get; set; }
    }
}

