using AutoMapper;
using FollowUP.Core.Domain;
using FollowUP.Infrastructure.Commands;
using FollowUP.Infrastructure.DTO;

namespace FollowUP.Infrastructure.Mappers
{
    public static class AutoMapperConfig
    {
        public static IMapper Initialize()
            => new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AccountSettings, AccountSettingsDto>();
                cfg.CreateMap<Comment, CommentDto>();
                cfg.CreateMap<FollowPromotion, PromotionDto>();
                cfg.CreateMap<InstagramAccount, AdminExtendedAccountDto>();
                cfg.CreateMap<InstagramAccount, ExtendedAccountDto>();
                cfg.CreateMap<InstagramAccount, InstagramAccountDto>();
                cfg.CreateMap<InstagramProxy, InstagramProxyDto>();
                cfg.CreateMap<JsonWebToken, JwtDto>();
                cfg.CreateMap<PromotionComment, PromotionCommentDto>();
                cfg.CreateMap<UpdateAccountSettings, AccountSettingsDto>();
                cfg.CreateMap<User, UserDto>();
            })
            .CreateMapper();
    }
}
