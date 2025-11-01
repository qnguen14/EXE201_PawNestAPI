using PawNest.Repository.Data.Requests.Review;
using PawNest.Repository.Data.Responses.Review;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.Services.Services.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<GetReviewResponse>> GetAll();
        Task<GetReviewResponse> GetById(Guid id);
        Task<GetReviewResponse> Create(CreateReviewRequest request);
        Task<GetReviewResponse> Update(Guid id, RespondReviewRequest request);
        Task<bool> Delete(Guid id);
    }
}
