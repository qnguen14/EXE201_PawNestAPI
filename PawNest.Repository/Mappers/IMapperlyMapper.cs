using PawNest.Repository.Data.Entities;
using PawNest.Repository.Data.Requests.Booking;
using PawNest.Repository.Data.Requests.Pet;
using PawNest.Repository.Data.Requests.Post;
using PawNest.Repository.Data.Requests.Review;
using PawNest.Repository.Data.Requests.Service;
using PawNest.Repository.Data.Requests.User;
using PawNest.Repository.Data.Responses.Booking;
using PawNest.Repository.Data.Responses.Pet;
using PawNest.Repository.Data.Responses.Post;
using PawNest.Repository.Data.Responses.Profile;
using PawNest.Repository.Data.Responses.Review;
using PawNest.Repository.Data.Responses.Service;
using PawNest.Repository.Data.Responses.User;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.Repository.Mappers
{
    public interface IMapperlyMapper
    {
        // Booking Mappers

        Booking MapToBooking(CreateBookingRequest request);

        // UpdateBookingRequest to Booking
        Booking UpdateBookingToBooking(UpdateBookingRequest request, Booking booking);

        // Booking to GetBookingResponse
        GetBookingResponse MapToGetBookingResponse(Booking booking);

        // Booking to GetBookingUpdateResponse
        GetBookingUpdateResponse MapToGetBookingUpdateResponse(Booking booking);

        // Pet to CreatePetRequest for response
        CreatePetRequest MapToCreatePetRequest(Pet pet);

        // CreatePetRequest to Pet
        Pet MapToPet(CreatePetRequest request);

        // AddPetRequest to Pet
        Pet AddRequestMapToPet(AddPetRequest request);

        // UpdatedPetRequest to Pet (for updating)
        GetPetResponse MapToGetPetResponse(Pet pet);

        // Pet to CreatePetResponse
        CreatePetResponse MapToCreatePetResponse(Pet pet);

        // IEnumerable mapping
        IEnumerable<CreatePetResponse> MapToCreatePetResponseList(IEnumerable<Pet> pets);

        // For update scenario - map request to existing pet
        void UpdatePetFromRequest(Pet source, Pet target);

        // Post Mappers

        // CreatePostRequest to Post
        Post MapToPost(CreatePostRequest request);

        // Post to CreatePostResponse
        CreatePostResponse MapToCreatePostResponse(Post post);

        // IEnumerable mapping
        IEnumerable<CreatePostResponse> MapToCreatePostResponseList(IEnumerable<Post> posts);

        // For update scenario
        void UpdatePostFromRequest(Post source, Post target);

        // Profile Mappers

        GetUserProfile MapToGetUserProfile(User user);

        GetFreelancerProfile MapToGetFreelancerProfile(User user);

        // Service Mappers


        // CreateServiceRequest to Service
        Service MapToService(CreateServiceRequest request);

        // Service to GetServiceResponse
        [MapProperty(nameof(Service.ServiceId), nameof(GetServiceResponse.Id))]
        GetServiceResponse MapToGetServiceResponse(Service service);

        // IEnumerable mapping
        IEnumerable<GetServiceResponse> MapToGetServiceResponseList(IEnumerable<Service> services);

        // For update - map request to existing service
        void UpdateServiceFromRequest(UpdateServiceRequest request, Service target);

        // User Mappers


        // CreateUserRequest to User
        User MapToUser(CreateUserRequest request);

        // User to CreateUserResponse
        [MapProperty(nameof(User.Role.RoleName), nameof(CreateUserResponse.Role))]
        CreateUserResponse MapToCreateUserResponse(User user);

        // User to GetFreelancerResponse
        [MapProperty(nameof(User.Role.RoleName), nameof(GetFreelancerResponse.Role))]
        GetFreelancerResponse MapToGetFreelancerResponse(User user);

        // Review Mappers

        Review MapToReview(CreateReviewRequest request);
        Review RespondMapToReview(RespondReviewRequest request);
        GetReviewResponse MapToGetReviewResponse(Review review);
    }
}
