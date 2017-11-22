using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class Purchase
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Person { get; set; }

        [Required]
        [StringLength(50)]
        public string UserId { get; set; }

        [Required]
        [StringLength(150)]
        public string Address { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [ForeignKey(nameof(BookId))]
        public virtual Book MyBook { get; set; }
    }
}
