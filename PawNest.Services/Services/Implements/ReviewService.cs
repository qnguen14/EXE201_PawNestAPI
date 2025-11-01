using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PawNest.Repository.Data.Context;
using PawNest.Repository.Data.Entities;
using PawNest.Repository.Data.Requests.Review;
using PawNest.Repository.Data.Responses.Review;
using PawNest.Repository.Mappers;
using PawNest.Repository.Repositories.Interfaces;
using PawNest.Services.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.Services.Services.Implements
{
    public class ReviewService : BaseService<ReviewService>, IReviewService
    {
        public ReviewService(IUnitOfWork<PawNestDbContext> unitOfWork,
             ILogger<ReviewService> logger,
             IMapperlyMapper mapper,
             IHttpContextAccessor httpContextAccessor,
             IUserService userService)
            : base(unitOfWork, logger, httpContextAccessor, mapper)
        {
        }

        public async Task<GetReviewResponse> Create(CreateReviewRequest request)
        {
            try
            {
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    if (request.Rating < 1 || request.Rating > 5)
                    {
                        throw new ArgumentException("Rating must be between 1 and 5");
                    }

                    if (request == null)
                    {
                        throw new NullReferenceException("Request cannot be null");
                    }

                    var newReview = _mapper.MapToReview(request);
                    var reviewRepo = _unitOfWork.GetRepository<Review>();
                    await reviewRepo.InsertAsync(newReview);

                    return _mapper.MapToGetReviewResponse(newReview);
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating review");
                throw;
            }
        }

        public async Task<bool> Delete(Guid id)
        {
            try
            {
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var reviewRepo = _unitOfWork.GetRepository<Review>();
                    var review = await reviewRepo.FirstOrDefaultAsync(predicate: x => x.Id == id);
                    reviewRepo.DeleteAsync(review);

                    return true;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating review");
                throw;
            }
        }

        public Task<IEnumerable<GetReviewResponse>> GetAll()
        {
            try
            {
                return _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var reviewRepo = _unitOfWork.GetRepository<Review>();
                    var reviews = await reviewRepo.GetListAsync();
                    return reviews.Select(r => _mapper.MapToGetReviewResponse(r));
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all reviews");
                throw;
            }
        }

        public Task<GetReviewResponse> GetById(Guid id)
        {
            try
            {
                return _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var reviewRepo = _unitOfWork.GetRepository<Review>();
                    var review = await reviewRepo.FirstOrDefaultAsync(predicate: x => x.Id == id);
                    return _mapper.MapToGetReviewResponse(review);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all reviews");
                throw;
            }
        }

        public async Task<GetReviewResponse> Update(Guid id, RespondReviewRequest request)
        {
            try
            {
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var existingReview = _mapper.RespondMapToReview(request);
                    var reviewRepo = _unitOfWork.GetRepository<Review>();
                    reviewRepo.UpdateAsync(existingReview);

                    return _mapper.MapToGetReviewResponse(existingReview);
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating review");
                throw;
            }
        }
    }
}
