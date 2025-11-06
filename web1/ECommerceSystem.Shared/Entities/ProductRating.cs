using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceSystem.Shared.Entities
{
    [Table("ProductRatings")]
    public class ProductRating
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        // Lưu user identifier dưới dạng string để tương thích nhiều kiểu auth (GUID, username, sub, ...)
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Range(1, 5)]
        public int Value { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Product? Product { get; set; }
    }
}
