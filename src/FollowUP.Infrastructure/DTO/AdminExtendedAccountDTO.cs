namespace FollowUP.Infrastructure.DTO
{
    public class AdminExtendedAccountDto : ExtendedAccountDto
    {
        public UserDto User { get; set; }
        public InstagramProxyDto InstagramProxy { get; set; }
    }
}
