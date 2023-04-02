using AspNetCoreIdentity.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreIdentity.Context
{
    public class IdentityContext:IdentityDbContext<AppUser,AppRole,int>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options):base(options)
        {

        }
    }
}
