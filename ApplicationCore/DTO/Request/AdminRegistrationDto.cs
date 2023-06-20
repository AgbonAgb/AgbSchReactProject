//using Infrastructure.Entity;
using Infrastructure.Accounts;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Accounts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTO.Request
{
    public class AdminRegistrationDto
    {

        [EmailAddress]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "The email is not valid")]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; set; }

        //[ValidatePhoneNumber(ErrorMessage = "Phone number with 11 digits beginning with 0 required")]
        [Required(ErrorMessage = "Phone Number  is required")]
        public string PhoneNumber { get; set; }

        public List<AdminRole> RoleName { get; set; }
       
    }
}
