using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookStore.ViewModels
{
    public class PurchaseViewModel
    {
        [Required]
        [StringLength(100)]
        public string Person { get; set; }

        [Required]
        [StringLength(150)]
        public string Address { get; set; }
    }
}