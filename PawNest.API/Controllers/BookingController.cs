﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PawNest.API.Constants;
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

        [HttpGet(ApiEndpointConstants.Booking.GetAllBookingsEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<GetBookingResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin, Staff, Customer, Freelancer")]
        public async Task<IActionResult> GetAllBookings()
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

            var apiReponse = new ApiResponse<IEnumerable<GetBookingResponse>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Bookings retrieved successfully.",
                IsSuccess = true,
                Data = bookings
            };

            return Ok(apiReponse);
        }


        [HttpGet(ApiEndpointConstants.Booking.GetBookingByIdEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<GetBookingResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin, Staff, Customer, Freelancer")]
        public async Task<IActionResult> GetBookingById([FromRoute] Guid bookingId)
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
            var apiReponse = new ApiResponse<GetBookingResponse>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Booking retrieved successfully.",
                IsSuccess = true,
                Data = booking
            };
            return Ok(apiReponse);
        }


        [HttpPost(ApiEndpointConstants.Booking.CreateBookingEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<GetBookingResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin, Staff, Customer")]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Invalid booking data.",
                });
            }
            var createdBooking = await _bookingService.CreateBookingAsync(request);
            var apiReponse = new ApiResponse<GetBookingResponse>
            {
                StatusCode = StatusCodes.Status201Created,
                Message = "Booking created successfully.",
                IsSuccess = true,
                Data = createdBooking
            };
            return Ok(apiReponse);
        }

        [HttpPut(ApiEndpointConstants.Booking.UpdateBookingEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<GetBookingUpdateResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin, Staff, Customer")]
        public async Task<IActionResult> UpdateBooking([FromRoute] Guid bookingId, [FromBody] UpdateBookingRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Invalid booking data.",
                });
            }
            try
            {
                var updatedBooking = await _bookingService.UpdateBookingAsync(bookingId, request);
                var apiReponse = new ApiResponse<GetBookingUpdateResponse>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Booking updated successfully.",
                    IsSuccess = true,
                    Data = updatedBooking
                };
                return Ok(apiReponse);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = $"Booking with ID {bookingId} not found."
                });
            }
        }

        [HttpPut(ApiEndpointConstants.Booking.CancelBookingEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin, Staff, Customer")]
        public async Task<IActionResult> CancelBooking([FromRoute] Guid bookingId)
        {
            try
            {
                var result = await _bookingService.CancelBookingAsync(bookingId);
                if (!result)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = $"Booking with ID {bookingId} not found."
                    });
                }
                var apiReponse = new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Booking canceled successfully.",
                    IsSuccess = true,
                    Data = null
                };
                return Ok(apiReponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while canceling the booking: " + ex.Message
                });
            }
        }
    }
}
