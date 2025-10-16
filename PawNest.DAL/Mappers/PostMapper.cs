using AutoMapper;
using PawNest.DAL.Data.Entities;
using PawNest.DAL.Data.Requests.Post;
using PawNest.DAL.Data.Responses.Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.DAL.Mappers
{
    public class PostMapper : Profile
    {
        public PostMapper()
        {
            CreateMap<CreatePostRequest, Post>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<Post, CreatePostResponse>()
                .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.PostStatus, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.PostCategory, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.StaffId, opt => opt.MapFrom(src => src.StaffId));
        }
    }
}
