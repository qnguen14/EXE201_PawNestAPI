using PawNest.Repository.Data.Entities;
using PawNest.Repository.Data.Requests.Booking;
using PawNest.Repository.Data.Requests.Pet;
using PawNest.Repository.Data.Requests.Post;
using PawNest.Repository.Data.Requests.Profile;
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
using PawNest.Repository.Data.Responses.Image;
using PawNest.Repository.Data.Responses.Video;

namespace PawNest.Repository.Mappers
{
    public interface IMapperlyMapper
    {
        // Video Mappers
        VideoResponse MapToVideoResponse(Video video);
        IEnumerable<VideoResponse> MapToVideoResponseList(IEnumerable<Video> videos);
        
        
        // Image Mappers
        ImageResponse MapToImageResponse(Image image);
        IEnumerable<ImageResponse> MapToImageResponseList(IEnumerable<Image> images);
        
        // Booking Mappers

        Booking MapToBooking(CreateBookingRequest request);

        // UpdateBookingRequest to Booking
        Booking UpdateBookingToBooking(UpdateBookingRequest request, Booking booking);

        // Booking to GetBookingResponse
        GetBookingResponse MapToGetBookingResponse(Booking booking);

        // Booking to GetBookingUpdateResponse
        GetBookingUpdateResponse MapToGetBookingUpdateResponse(Booking booking);

        BookingServiceResponse MapToBookingServiceResponse(Service service);
        BookingPetResponse MapToBookingPetResponse(Pet pet);

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
        // UpdatePetRequest to Pet (for updating existing pet)
        void UpdatePetFromRequest(UpdatePetRequest request, Pet target);

        // EditPetRequest to Pet (if you need this one too)
        void UpdatePetFromEditRequest(EditPetRequest request, Pet target);


        // Post Mappers

        // CreatePostRequest to Post
        Post MapToPost(CreatePostRequest request);

        // Post to CreatePostResponse
        CreatePostResponse MapToCreatePostResponse(Post post);

        // IEnumerable mapping
        IEnumerable<CreatePostResponse> MapToCreatePostResponseList(IEnumerable<Post> posts);

        // For update scenario
        void UpdatePostFromRequest(UpdatePostRequest request, Post target);

        // Profile Mappers

        GetUserProfile MapToGetUserProfile(User user);

        GetFreelancerProfile MapToGetFreelancerProfile(User user);
        void UpdateUserProfileFromRequest(UpdateUserProfileRequest request, User target);
        void UpdateFreelancerProfileFromRequest(UpdateFreelancerProfileRequest request, User target);

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
        GetUserProfile MapToGetUserProfileWithBookings(User user);
        GetFreelancerProfile MapToGetFreelancerProfileWithBookings(User user);

    }
}
