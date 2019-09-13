using FollowUP.Infrastructure.DTO;
using System;

namespace FollowUP.Infrastructure.Services
{
    public interface IJwtHandler
    {
        JwtDto CreateToken(Guid userId, string role);
    }
}
