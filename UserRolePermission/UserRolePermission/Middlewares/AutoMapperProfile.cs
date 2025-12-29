using AutoMapper;
using UserRolePermission.Common.Models;
using UserRolePermission.Dto;
using UserRolePermission.DTO;
using UserRolePermission.Repository.Models;

namespace UserRolePermission.Middlewares
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserDto, User>().ReverseMap();
            CreateMap<User, UserDb>().ReverseMap();
            CreateMap<RoleDto, Role>().ReverseMap();
            CreateMap<Role, RoleDb>().ReverseMap();
            CreateMap<StatusDto, Status>().ReverseMap();
            CreateMap<Status, StatusDb>().ReverseMap();
            CreateMap<ScreenDto, Screen>().ReverseMap();
            CreateMap<Screen, ScreenDb>().ReverseMap();
            CreateMap<ScreenActionDto, ScreenAction>().ReverseMap();
            CreateMap<ScreenAction, ScreenActionDb>().ReverseMap();
            CreateMap<MenuStructureDto, MenuStructure>().ReverseMap();
            CreateMap<MenuStructure, MenuStructureDb>().ReverseMap();
            CreateMap<MenuStructure, MenuItemDto>()
                .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.ActionKey))
                .ForMember(dest => dest.Label, opt => opt.MapFrom(src => src.Label))
                .ForMember(dest => dest.Icon, opt => opt.MapFrom(src => src.Icon))
                .ForMember(dest => dest.Link, opt => opt.MapFrom(src => src.RoutePath))
                .ForMember(dest => dest.IsTitle, opt => opt.MapFrom(src => src.IsTitle));
            CreateMap<RolePermissionDto, RolePermission>().ReverseMap();
            CreateMap<RolePermission, RolePermissionDb>().ReverseMap();
            CreateMap<UserPermissionOverrideDto, UserPermissionOverride>().ReverseMap();
            CreateMap<UserPermissionOverride, UserPermissionOverrideDb>().ReverseMap();
        }
    }
}
