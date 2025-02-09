using System;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Final.Models
{
    public class Category
    {
        [Key]
        public Guid CategoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string CategoryName { get; set; } = string.Empty;

        public string Image { get; set; }
    }
}
