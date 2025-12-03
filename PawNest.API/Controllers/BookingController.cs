using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PawNest.API.Constants;
using PawNest.Repository.Data.Entities;
using PawNest.Services.Services.Interfaces;
using PawNest.Repository.Data.Metadata;
using PawNest.Repository.Data.Requests.Booking;
using PawNest.Repository.Data.Responses.Booking;

namespace PawNest.API.Controllers
{
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Lấy tất cả các booking 
        /// </summary>
        [HttpGet(ApiEndpointConstants.Booking.GetAllBookingsEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<GetBookingResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin, Staff, Freelancer")]
        public async Task<IActionResult> GetAllBookings()
        {
            try
            {
                var bookings = await _bookingService.GetAllBookingsAsync();
                if (bookings == null || !bookings.Any())
                {
                    return NotFound(new ApiResponse<object>
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "No bookings found."
                    });
                }

                var apiResponse = new ApiResponse<IEnumerable<GetBookingResponse>>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Bookings retrieved successfully.",
                    IsSuccess = true,
                    Data = bookings
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while retrieving bookings: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy booking theo ID
        /// </summary>
        [HttpGet(ApiEndpointConstants.Booking.GetBookingByIdEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<GetBookingResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<IActionResult> GetBookingById([FromRoute] Guid bookingId)
        {
            try
            {
                var booking = await _bookingService.GetBookingByIdAsync(bookingId);
                if (booking == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = $"Booking with ID {bookingId} not found."
                    });
                }

                var apiResponse = new ApiResponse<GetBookingResponse>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Booking retrieved successfully.",
                    IsSuccess = true,
                    Data = booking
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while retrieving the booking: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy chi tiết booking theo ID (bao gồm đầy đủ thông tin payment, services, pets)
        /// </summary>
        [HttpGet(ApiEndpointConstants.Booking.GetBookingDetailsEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<GetBookingResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<IActionResult> GetBookingDetails([FromRoute] Guid bookingId)
        {
            try
            {
                var booking = await _bookingService.GetBookingDetailsAsync(bookingId);
                if (booking == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = $"Booking with ID {bookingId} not found."
                    });
                }

                var apiResponse = new ApiResponse<GetBookingResponse>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Booking details retrieved successfully.",
                    IsSuccess = true,
                    Data = booking
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while retrieving booking details: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy lịch sử booking của customer hiện tại
        /// </summary>
        [HttpGet(ApiEndpointConstants.Booking.GetMyBookingHistoryEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<GetBookingResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyBookingHistory()
        {
            try
            {
                var bookings = await _bookingService.GetMyBookingHistoryAsync();
                if (bookings == null || !bookings.Any())
                {
                    return NotFound(new ApiResponse<object>
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "No booking history found."
                    });
                }

                var apiResponse = new ApiResponse<IEnumerable<GetBookingResponse>>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Booking history retrieved successfully.",
                    IsSuccess = true,
                    Data = bookings
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while retrieving booking history: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy lịch sử booking của customer theo ID
        /// </summary>
        [HttpGet(ApiEndpointConstants.Booking.GetBookingHistoryByCustomerEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<GetBookingResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> GetBookingHistoryByCustomer([FromRoute] Guid customerId)
        {
            try
            {
                var bookings = await _bookingService.GetBookingHistoryByCustomerAsync(customerId);
                if (bookings == null || !bookings.Any())
                {
                    return NotFound(new ApiResponse<object>
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = $"No booking history found for customer ID {customerId}."
                    });
                }

                var apiResponse = new ApiResponse<IEnumerable<GetBookingResponse>>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Customer booking history retrieved successfully.",
                    IsSuccess = true,
                    Data = bookings
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while retrieving customer booking history: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy lịch sử booking của freelancer theo ID
        /// </summary>
        [HttpGet(ApiEndpointConstants.Booking.GetBookingHistoryByFreelancerEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<GetBookingResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin, Staff, Freelancer")]
        public async Task<IActionResult> GetBookingHistoryByFreelancer([FromRoute] Guid freelancerId)
        {
            try
            {
                var bookings = await _bookingService.GetBookingHistoryByFreelancerAsync(freelancerId);
                if (bookings == null || !bookings.Any())
                {
                    return NotFound(new ApiResponse<object>
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = $"No booking history found for freelancer ID {freelancerId}."
                    });
                }

                var apiResponse = new ApiResponse<IEnumerable<GetBookingResponse>>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Freelancer booking history retrieved successfully.",
                    IsSuccess = true,
                    Data = bookings
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while retrieving freelancer booking history: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Tạo booking mới
        /// </summary>
        [HttpPost(ApiEndpointConstants.Booking.CreateBookingEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<GetBookingResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Invalid booking data."
                    });
                }

                var createdBooking = await _bookingService.CreateBookingAsync(request);
                var apiResponse = new ApiResponse<GetBookingResponse>
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = "Booking created successfully.",
                    IsSuccess = true,
                    Data = createdBooking
                };

                return Created($"/api/bookings/{createdBooking.BookingId}", apiResponse);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while creating the booking: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Cập nhật trạng thái thanh toán của booking theo ID
        /// </summary>
        [HttpPut(ApiEndpointConstants.Booking.UpdateBookingEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<GetBookingUpdateResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> UpdateBooking([FromRoute] Guid bookingId, [FromBody] UpdateBookingRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Invalid booking data."
                    });
                }

                var updatedBooking = await _bookingService.UpdateBookingAsync(bookingId, request);
                var apiResponse = new ApiResponse<GetBookingUpdateResponse>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Booking payment status updated successfully.",
                    IsSuccess = true,
                    Data = updatedBooking
                };

                return Ok(apiResponse);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while updating the booking: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Hủy booking theo ID
        /// </summary>
        [HttpPut(ApiEndpointConstants.Booking.CancelBookingEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> CancelBooking([FromRoute] Guid bookingId)
        {
            try
            {
                var result = await _bookingService.CancelBookingAsync(bookingId);

                var apiResponse = new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Booking cancelled successfully.",
                    IsSuccess = true,
                    Data = null
                };

                return Ok(apiResponse);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while cancelling the booking: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Cập nhật trạng thái booking theo ID
        /// </summary>
        [HttpPut(ApiEndpointConstants.Booking.UpdateBookingStatusEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin, Staff, Freelancer")]
        public async Task<IActionResult> UpdateBookingStatus([FromRoute] Guid id, [FromQuery] BookingStatus status)
        {
            try
            {
                var result = await _bookingService.UpdateBookingStatusAsync(id, status);

                var apiResponse = new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = $"Booking status updated to {status} successfully.",
                    IsSuccess = true,
                    Data = null
                };

                return Ok(apiResponse);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while updating the booking status: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Cập nhật trạng thái lấy pet của booking theo ID
        /// </summary>
        [HttpPut(ApiEndpointConstants.Booking.UpdateBookingPickUpStatusEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin, Staff, Freelancer")]
        public async Task<IActionResult> UpdateBookingPickUpStatus([FromRoute] Guid id, [FromQuery] PickUpStatus status)
        {
            try
            {
                var result = await _bookingService.UpdateBookingPickUpStatusAsync(id, status);

                var apiResponse = new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = $"Booking's pick up status updated to {status} successfully.",
                    IsSuccess = true,
                    Data = null
                };

                return Ok(apiResponse);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while updating the booking pick up status: " + ex.Message
                });
            }
        }
    }
}