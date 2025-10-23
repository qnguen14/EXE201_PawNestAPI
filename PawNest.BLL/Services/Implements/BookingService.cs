
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PawNest.BLL.Services.Interfaces;
using PawNest.DAL.Data.Context;
using PawNest.DAL.Data.Entities;
using PawNest.DAL.Data.Requests.Booking;
using PawNest.DAL.Data.Responses.Booking;
using PawNest.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PawNest.DAL.Mappers;

namespace PawNest.BLL.Services.Implements
{
    public class BookingService : BaseService<BookingService>, IBookingService
    {
        private readonly BookingMapper _bookingMapper;
        public BookingService(IUnitOfWork<PawNestDbContext> unitOfWork, ILogger<BookingService> logger, IHttpContextAccessor httpContextAccessor)
            : base(unitOfWork, logger, httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        private bool IsCustomer(string role)
        {
            if (role != "Customer")
            {
                return false;
            } else
            {
                return true;
            }
        }

        public async Task<GetBookingResponse> CreateBookingAsync(CreateBookingRequest request)
        {
            try
            {
                var userRole = GetCurrentUserRole();
                IsCustomer(userRole);

                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var existingBooking = await _unitOfWork.GetRepository<Booking>()
                        .FirstOrDefaultAsync(
                        predicate: b => b.ServiceId == request.ServiceId && 
                                   b.FreelancerId == request.FreelancerId && 
                                   b.BookingDate == request.BookingDate,
                        null,
                        include: b => b.Include(booking => booking.Freelancer)
                                       .Include(booking => booking.Customer)
                                       .Include(booking => booking.Service)
                        );

                    if (existingBooking != null)
                    {
                        _logger.LogWarning("A booking already exists for the specified service, freelancer, customer, and date.");
                    }

                    var booking = _bookingMapper.MapToBooking(request);
                    booking.CustomerId = GetCurrentUserId();
                    booking.Status = BookingStatus.Pending;

                    await _unitOfWork.GetRepository<Booking>().InsertAsync(booking);

                    return _bookingMapper.MapToGetBookingResponse(booking);


                });
            } catch (Exception ex)
            {
                _logger.LogError("An error occurred while creating the booking: " + ex.Message);
                throw;
            }
        }

        public async Task<bool> CancelBookingAsync(Guid bookingId)
        {
            try
            {
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var booking = await _unitOfWork.GetRepository<Booking>()
                        .FirstOrDefaultAsync(predicate: b => b.BookingId == bookingId, include: b => b.Include(u => u.Freelancer).Include(u => u.Customer));
                    if (booking == null)
                    {
                        throw new KeyNotFoundException("Booking with ID " + bookingId + " not found.");
                    }

                    booking.Status = BookingStatus.Cancelled;

                    return true;
                });

            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while cancelling the booking: " + ex.Message);
                throw;
            }
        }

        // Implement methods related to booking operations
        public async Task<IEnumerable<GetBookingResponse>> GetAllBookingsAsync()
        {
            try
            {
                var bookings = await _unitOfWork.GetRepository<Booking>()
                    .GetListAsync(null,
                                  include: b => b.Include(u => u.Service)
                                                 .Include(u => u.Freelancer)
                                                 .Include(u => u.Customer),
                                  orderBy: b => b.OrderBy(u => u.BookingId));
                 if (bookings == null || !bookings.Any())
                {
                    return null;
                }

                var result = bookings.Select(b => _bookingMapper.MapToGetBookingResponse(b));
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving bookings: " + ex.Message);
                throw;
            }
        }

        public async Task<GetBookingResponse> GetBookingByIdAsync(Guid bookingId)
        {
            try
            {
                var booking = await _unitOfWork.GetRepository<Booking>()
                    .FirstOrDefaultAsync(
                        predicate: b => b.BookingId == bookingId,
                        include: b => b.Include(u => u.Service)
                                       .Include(u => u.Freelancer)
                                       .Include(u => u.Customer)
                    );

                if (booking == null)
                {
                    return null;
                }

                return _bookingMapper.MapToGetBookingResponse(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving the booking: " + ex.Message);
                throw;
            }
        }

        public async Task<GetBookingUpdateResponse> UpdateBookingAsync(Guid bookingId, UpdateBookingRequest request)
        {
            try
            {
                var booking = await _unitOfWork.GetRepository<Booking>()
                    .FirstOrDefaultAsync(predicate: b => b.BookingId == bookingId, include: b => b.Include(u => u.Freelancer).Include(u => u.Customer));
                if (booking == null)
                {
                    throw new KeyNotFoundException("Booking with ID " + bookingId + " not found.");
                }

                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var booking = await _unitOfWork.GetRepository<Booking>()
                        .FirstOrDefaultAsync(predicate: b => b.BookingId == bookingId, include: b => b.Include(u => u.Freelancer).Include(u => u.Customer));
                    if (booking == null)
                    {
                        throw new KeyNotFoundException("Booking with ID " + bookingId + " not found.");
                    }
                    // Update the booking entity with new values from the request
                    booking.Status = request.Status;
                    booking.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.GetRepository<Booking>().UpdateAsync(booking);
                    return _bookingMapper.MapToGetBookingUpdateResponse(booking);
                });
            } catch (Exception ex)
            {
                _logger.LogError("An error occurred while updating the booking: " + ex.Message);
                throw;
            }
        }
    }
}
