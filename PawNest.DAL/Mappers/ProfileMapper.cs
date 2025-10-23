    using AutoMapper;
    using PawNest.DAL.Data.Entities;
    using PawNest.DAL.Data.Requests.User;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using PawNest.DAL.Data.Responses.Profile;

    namespace PawNest.DAL.Mappers
    {
        public class ProfileMapper: Profile
        {
            public ProfileMapper() 
            {
                // Map từ User -> UserProfileDto (hiển thị thông tin)
                CreateMap<User, GetUserProfile>()
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                    .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                    .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
                    .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                    .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName))
                    .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

           
            }
        }
    }
