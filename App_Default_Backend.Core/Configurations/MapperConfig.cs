using App_Default_Backend.Core.Models;
using App_Default_Backend.Data.Models;
using AutoMapper;

namespace App_Default_Backend.Core.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {   
            CreateMap<User, InputUser>().ReverseMap()
                .AfterMap((inusr, usr) => usr.UserName= inusr.Email);
            CreateMap<User, OutputUser>().ReverseMap();
            CreateMap<Todo, InputTodo>().ReverseMap();
            CreateMap<Todo, OutputTodo>().ReverseMap();
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>)).ForMember("Items", opt => opt.MapFrom("Items"));

        }
    }
}
