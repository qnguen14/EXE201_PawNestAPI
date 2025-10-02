using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.DAL.Data.Entities
{
    public class Report
    {
        [Key]
        public Guid ReportId { get; set; }
        public string Reason { get; set; } = null!;
        public string? Details { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // FK
        public Guid ReporterId { get; set; }
        public User Reporter { get; set; }
    }
}
