using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime BirthDate { get; set; }

        public ApplicationUser()
        {
        }
    }
}