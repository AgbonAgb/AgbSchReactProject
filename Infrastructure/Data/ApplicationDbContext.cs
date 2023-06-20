using Infrastructure.Accounts;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    //<ApplicationUser, ApplicationRole, string>
    //public class ApplicationDbContext: IdentityDbContext
    public class ApplicationDbContext: IdentityDbContext<AdminRegistration, AdminRole, string>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {

        }

    }
}
