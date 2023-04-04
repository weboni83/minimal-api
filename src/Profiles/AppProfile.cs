using AutoMapper;
using MinimalAPIsDemo.DTOs;
using MinimalAPIsDemo.Entities;

namespace MinimalAPIsDemo.Profiles
{
    public class AppProfile : Profile
    {
        public AppProfile()
        {
            CreateMap<SYS_USER_INFO, UserDTO>();
            CreateMap<UserDTO, SYS_USER_INFO>();
            CreateMap<SYS_ALARM, AlarmDTO>();
            CreateMap<AlarmDTO, SYS_ALARM>();
            CreateMap<CM_COMMON, CommonDTO>();
            CreateMap<CommonDTO, CM_COMMON>();

            //CreateMap<SYS_ALARM, AlarmDTO>()
            //    .ForMember(dest => dest.ALARM_TITLE
            //    , opt => opt.MapFrom(src => src.FORM_NAME));
        }
    }
}
