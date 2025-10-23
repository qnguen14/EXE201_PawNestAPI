using Riok.Mapperly.Abstractions;
using PawNest.DAL.Data.Entities;
using PawNest.DAL.Data.Requests.Booking;
using PawNest.DAL.Data.Requests.Pet;
using PawNest.DAL.Data.Responses.Booking;

namespace PawNest.DAL.Mappers;

[Mapper]
public partial class BookingMapper
{
    // CreateBookingRequest to Booking
    public partial Booking MapToBooking(CreateBookingRequest request);

    // UpdateBookingRequest to Booking
    public partial Booking MapToBooking(UpdateBookingRequest request, Booking booking);

    // Booking to GetBookingResponse
    public partial GetBookingResponse MapToGetBookingResponse(Booking booking);

    // Booking to GetBookingUpdateResponse
    public partial GetBookingUpdateResponse MapToGetBookingUpdateResponse(Booking booking);

    // Pet to CreatePetRequest for response
    public partial CreatePetRequest MapToCreatePetRequest(Pet pet);

    // CreatePetRequest to Pet
    public partial Pet MapToPet(CreatePetRequest request);
}