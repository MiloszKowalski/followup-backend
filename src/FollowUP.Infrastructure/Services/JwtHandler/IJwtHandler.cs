using FollowUP.Infrastructure.DTO;
using System;

namespace FollowUP.Infrastructure.Services
{
    public interface IJwtHandler : IService
    {
        JwtDto CreateToken(Guid userId, string role);
    }
}
