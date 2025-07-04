using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Government.Domain.Entities.Liscenses
{
    public class  ServicePrice
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public  string  ServiceCode { get; set; }

        [Required]
        [MaxLength(100)]
        public string ServiceName { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [MaxLength(3)]
        public string Currency { get; set; } = "EGP";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
