using AutoMapper;
using PawNest.DAL.Data.Entities;
using PawNest.DAL.Data.Requests.Booking;
using PawNest.DAL.Data.Requests.Pet;
using PawNest.DAL.Data.Responses.Booking;

namespace PawNest.DAL.Mappers
{
    public class BookingMapper : Profile
    {
        public BookingMapper()
        {
            // CreateBookingRequest to Booking
            CreateMap<CreateBookingRequest, Booking>()
                .ForMember(dest => dest.FreelancerId, opt => opt.Ignore()) // This should be set from Service
                .ForMember(dest => dest.Freelancer, opt => opt.Ignore())
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Service, opt => opt.Ignore())
                .ForMember(dest => dest.Pets, opt => opt.Ignore()); // Handle pets separately in service layer

            // UpdateBookingRequest to Booking
            CreateMap<UpdateBookingRequest, Booking>()
                .ForMember(dest => dest.BookingId, opt => opt.Ignore()) // BookingId should not be updated
                .ForMember(dest => dest.FreelancerId, opt => opt.Ignore()) // This should be set from Service
                .ForMember(dest => dest.CustomerId, opt => opt.Ignore()) // Customer should not change via update
                .ForMember(dest => dest.Freelancer, opt => opt.Ignore())
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Service, opt => opt.Ignore())
                .ForMember(dest => dest.Pets, opt => opt.Ignore()); // Handle pets separately in service layer

            // Booking to GetBookingResponse
            CreateMap<Booking, GetBookingResponse>()
                .ForMember(dest => dest.Pets, opt => opt.MapFrom(src => src.Pets));


            // Booking to GetBookingUpdateResponse
            CreateMap<Booking, GetBookingUpdateResponse>()
                .ForMember(dest => dest.Pets, opt => opt.MapFrom(src => src.Pets));

            // Pet to CreatePetRequest for the response
            CreateMap<Pet, CreatePetRequest>()
                .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.CustomerId));

            // CreatePetRequest to Pet (if needed for creating pets from booking)
            CreateMap<CreatePetRequest, Pet>()
                .ForMember(dest => dest.PetId, opt => opt.Ignore()) // Auto-generated
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.OwnerId))
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Bookings, opt => opt.Ignore());
        }
    }
}