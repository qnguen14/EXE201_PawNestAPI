using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.Repository.Data.Responses.Review
{
    public class GetReviewResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
        public string Response { get; set; }
        public double Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime RespondedAt { get; set; }
    }
}
