using AutoMapper;
using FollowUP.Core.Domain;
using FollowUP.Infrastructure.DTO;

namespace FollowUP.Infrastructure.Mappers
{
    public static class AutoMapperConfig
    {
        public static IMapper Initialize()
            => new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDto>();
            })
            .CreateMapper();
    }
}
