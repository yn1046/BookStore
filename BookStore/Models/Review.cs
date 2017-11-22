using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BookStore.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        [StringLength(1000)]
        public string Text { get; set; }

        [Required]
        public int Rating { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [ForeignKey(nameof(BookId))]
        public virtual Book MyBook { get; set; }
    }
}