using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreIdentity.Models
{
    public class UserAdminCreateModel
    {
        [Required(ErrorMessage = "Kullanıcı adı gereklidir")]
        public string Username { get; set; }
       
        [Required(ErrorMessage = "Email Adresi gereklidir")]
        public string Email { get; set; }
        
        
        [Required(ErrorMessage = "Gender gereklidir")]
        public string Gender { get; set; }
    }
}
