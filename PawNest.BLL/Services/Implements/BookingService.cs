
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
        private readonly IMapperlyMapper _bookingMapper;
        public BookingService(IUnitOfWork<PawNestDbContext> unitOfWork, ILogger<BookingService> logger, IHttpContextAccessor httpContextAccessor, IMapperlyMapper mapper)
            : base(unitOfWork, logger, httpContextAccessor, mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _bookingMapper = mapper;
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

        private decimal CalculateTotalPrice(decimal servicePrice, int petCount)
        {
            return servicePrice * petCount;
        }

        // Pseudocode:
        // - Validate current user role is Customer; else throw
        // - Validate request, ensure at least one ServiceId and one PetId
        // - Pick the first ServiceId (since Booking supports a single Service)
        // - In transaction:
        //   - Check if a booking exists for (serviceId, freelancerId, bookingDate); log warning if found
        //   - Load the Service to get its Price; throw if not found
        //   - Map request to Booking
        //   - Set CustomerId to current user, ServiceId to selected service
        //   - Compute TotalPrice = service.Price * number of pets
        //   - Insert booking
        //   - Map and return GetBookingResponse
        public async Task<GetBookingResponse> CreateBookingAsync(CreateBookingRequest request)
        {
            try
            {
                var userRole = GetCurrentUserRole();
                if (!IsCustomer(userRole))
                {
                    throw new UnauthorizedAccessException("Only customers can create bookings.");
                }

                if (request is null) throw new ArgumentNullException(nameof(request));
                if (request.ServiceIds is null || request.ServiceIds.Count == 0)
                    throw new ArgumentException("At least one service must be selected.", nameof(request.ServiceIds));
                if (request.PetIds is null || request.PetIds.Count == 0)
                    throw new ArgumentException("At least one pet must be selected.", nameof(request.PetIds));

                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    // Check for existing bookings with any of the requested services
                    var existingBooking = await _unitOfWork.GetRepository<Booking>()
                        .FirstOrDefaultAsync(
                            predicate: b => b.FreelancerId == request.FreelancerId &&
                                            b.BookingDate == request.BookingDate &&
                                            b.Services.Any(s => request.ServiceIds.Contains(s.ServiceId)),
                            orderBy: null,
                            include: b => b.Include(booking => booking.Freelancer)
                                           .Include(booking => booking.Customer)
                                           .Include(booking => booking.Services)
                        );

                    if (existingBooking != null)
                    {
                        _logger.LogWarning("A booking already exists for the specified services, freelancer, and date.");
                        throw new InvalidOperationException("A booking already exists for one or more of the requested services on this date.");
                    }

                    // Load only the requested services
                    var services = await _unitOfWork.GetRepository<Service>().GetListAsync(
                        predicate: s => request.ServiceIds.Contains(s.ServiceId) && s.FreelancerId == request.FreelancerId);

                    if (services == null || services.Count != request.ServiceIds.Count)
                    {
                        throw new KeyNotFoundException("One or more requested services were not found or do not belong to the specified freelancer.");
                    }

                    // Calculate total price for all services
                    decimal totalServicePrice = services.Sum(s => s.Price);

                    // Load the pets to attach to the booking
                    var pets = await _unitOfWork.GetRepository<Pet>().GetListAsync(
                        predicate: p => request.PetIds.Contains(p.PetId));

                    if (pets == null || pets.Count != request.PetIds.Count)
                    {
                        throw new KeyNotFoundException("One or more requested pets were not found.");
                    }

                    var booking = _bookingMapper.MapToBooking(request);
                    booking.CustomerId = GetCurrentUserId();
                    booking.TotalPrice = CalculateTotalPrice(totalServicePrice, request.PetIds.Count);
                    booking.Services = services.ToList();
                    booking.Status = BookingStatus.Pending;
                    booking.PickUpStatus = PickUpStatus.NotPickedUp;
                    booking.Pets = pets.ToList();

                    await _unitOfWork.GetRepository<Booking>().InsertAsync(booking);

                    return _bookingMapper.MapToGetBookingResponse(booking);
                });
            }
            catch (Exception ex)
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
                                  include: b => b.Include(u => u.Freelancer)
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
                        include: b => b.Include(u => u.Freelancer)
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
