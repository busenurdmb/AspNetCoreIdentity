using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreIdentity.Entities
{
    public class AppUser:IdentityUser<int>
    {
        public string ImagePath { get; set; }
        //3nfy aykırı normalizasyona aykırı  neden 
        //bir kullanıcının gender bilgisini tutucaksak string olarak tutmayız 
        //gender ayrı bir tabloda olmalı ben bir alanı güncellediğim zaman 
        //bütün tablomdaki userları güncellememem için 
        public string Gender { get; set; }
    }
}
