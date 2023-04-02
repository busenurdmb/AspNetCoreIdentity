using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreIdentity.Models
{
    public class UserCreateModel
    {
        [Required(ErrorMessage ="Kullanıcı adı gereklidir")]
        public string Username { get; set; }
        [EmailAddress(ErrorMessage ="Lütfen bir email formatı giriniz")]
        [Required(ErrorMessage = "Email Adresi gereklidir")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Parola Alanı gereklidir")]
        public string Password { get; set; }
        [Compare("Password",ErrorMessage ="Parolalar eşleşmiyor")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Gender gereklidir")]
        public string Gender { get; set; }
    }
}
