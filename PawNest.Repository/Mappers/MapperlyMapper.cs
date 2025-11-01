﻿using PawNest.Repository.Data.Entities;
using PawNest.Repository.Data.Requests.Booking;
using PawNest.Repository.Data.Requests.Pet;
using PawNest.Repository.Data.Requests.Post;
using PawNest.Repository.Data.Requests.Service;
using PawNest.Repository.Data.Requests.User;
using PawNest.Repository.Data.Responses.Booking;
using PawNest.Repository.Data.Responses.Pet;
using PawNest.Repository.Data.Responses.Post;
using PawNest.Repository.Data.Responses.Profile;
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
    [Mapper]
    public partial class MapperlyMapper : IMapperlyMapper
    {
        // Booking Mappers

        // CreateBookingRequest to Booking
        public partial Booking MapToBooking(CreateBookingRequest request);

        // UpdateBookingRequest to Booking
        public partial Booking UpdateBookingToBooking(UpdateBookingRequest request, Booking booking);

        // Booking to GetBookingResponse
        public partial GetBookingResponse MapToGetBookingResponse(Booking booking);

        // Booking to GetBookingUpdateResponse
        public partial GetBookingUpdateResponse MapToGetBookingUpdateResponse(Booking booking);

        // Pet to CreatePetRequest for response
        public partial CreatePetRequest MapToCreatePetRequest(Pet pet);

        // Pet Mappers

        // CreatePetRequest to Pet
        public partial Pet MapToPet(CreatePetRequest request);

        // UpdatedPetRequest to Pet (for updating)
        public partial Pet MapToPetFromUpdate(UpdatedPetRequest request);

        // Pet to CreatePetResponse
        public partial CreatePetResponse MapToCreatePetResponse(Pet pet);

        // IEnumerable mapping
        public partial IEnumerable<CreatePetResponse> MapToCreatePetResponseList(IEnumerable<Pet> pets);

        // For update scenario - map request to existing pet
        public partial void UpdatePetFromRequest(Pet source, Pet target);


        // Post Mappers

        // CreatePostRequest to Post
        public partial Post MapToPost(CreatePostRequest request);

        // Post to CreatePostResponse
        [MapProperty(nameof(Post.Id), nameof(CreatePostResponse.PostId))]
        [MapProperty(nameof(Post.Status), nameof(CreatePostResponse.PostStatus))]
        [MapProperty(nameof(Post.Category), nameof(CreatePostResponse.PostCategory))]
        public partial CreatePostResponse MapToCreatePostResponse(Post post);

        // IEnumerable mapping
        public partial IEnumerable<CreatePostResponse> MapToCreatePostResponseList(IEnumerable<Post> posts);

        // For update scenario
        public partial void UpdatePostFromRequest(Post source, Post target);

        // Profile Mappers


        [MapProperty(nameof(User.Role.RoleName), nameof(GetUserProfile.Role))]
        public partial GetUserProfile MapToGetUserProfile(User user);

        [MapProperty(nameof(User.Role.RoleName), nameof(GetFreelancerProfile.Role))]
        public partial GetFreelancerProfile MapToGetFreelancerProfile(User user);

        // Service Mappers


        // CreateServiceRequest to Service
        public partial Service MapToService(CreateServiceRequest request);

        // Service to GetServiceResponse
        [MapProperty(nameof(Service.ServiceId), nameof(GetServiceResponse.Id))]
        public partial GetServiceResponse MapToGetServiceResponse(Service service);

        // IEnumerable mapping
        public partial IEnumerable<GetServiceResponse> MapToGetServiceResponseList(IEnumerable<Service> services);

        // For update - map request to existing service
        public partial void UpdateServiceFromRequest(UpdateServiceRequest request, Service target);

        // User Mappers


        // CreateUserRequest to User
        [MapProperty(nameof(CreateUserRequest.Email), nameof(User.Email))]
        [MapProperty(nameof(CreateUserRequest.PhoneNumber), nameof(User.PhoneNumber))]
        [MapProperty(nameof(CreateUserRequest.Address), nameof(User.Address))]
        public partial User MapToUser(CreateUserRequest request);

        // User to CreateUserResponse
        [MapProperty(nameof(User.Role.RoleName), nameof(CreateUserResponse.Role))]
        public partial CreateUserResponse MapToCreateUserResponse(User user);

        // User to GetFreelancerResponse
        [MapProperty(nameof(User.Role.RoleName), nameof(GetFreelancerResponse.Role))]
        public partial GetFreelancerResponse MapToGetFreelancerResponse(User user);


    }
}
