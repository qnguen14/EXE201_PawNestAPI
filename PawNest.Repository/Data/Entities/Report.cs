using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.Repository.Data.Entities
{
    public class Report
    {
        [Key]
        public Guid ReportId { get; set; }

        [Required]
        public string Reason { get; set; } = null!;

        public string? Details { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key
        [ForeignKey("Staff")]
        public Guid StaffId { get; set; }
        public virtual User Staff { get; set; } = null!;
    }
}