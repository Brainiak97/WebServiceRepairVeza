using AutoMapper;
using BLL.Models;
using WebService.Models.ViewModels.Comment;
using WebService.Models.ViewModels.Notification;
using WebService.Models.ViewModels.RepairGroup;
using WebService.Models.ViewModels.RepairLog;
using WebService.Models.ViewModels.User;

namespace WebService.Models.Mapper
{
    public class PresentationProfile : Profile
    {
        public PresentationProfile()
        {
            CreateMap<CommentDto, CommentViewModel>().ReverseMap();
            CreateMap<RepairLogDto, RepairLogViewModel>().ReverseMap();
            CreateMap<RepairGroupDto, RepairGroupViewModel>().ReverseMap();
            CreateMap<UserDto, UserViewModel>().ReverseMap();
            CreateMap<NotificationDto, NotificationViewModel>().ReverseMap();
        }
    }
}