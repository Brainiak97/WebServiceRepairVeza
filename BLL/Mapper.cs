using AutoMapper;
using BLL.Models;
using Core.Models;

namespace BLL
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Comment, CommentDto>().ReverseMap();
            CreateMap<RepairGroup, RepairGroupDto>().ReverseMap();
            CreateMap<RepairLog, RepairLogDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Notification, NotificationDto>().ReverseMap();
        }
    }
}
