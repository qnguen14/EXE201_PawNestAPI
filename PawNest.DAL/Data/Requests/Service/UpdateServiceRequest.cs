using PawNest.DAL.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.DAL.Data.Requests.Service
{
    public class UpdateServiceRequest
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public ServiceType Type { get; set; }
        public decimal Price { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
