using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.Exceptions;
using System;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Extensions
{
    public static class RepositoryExtensions
    { 
        public static async Task<User> GetOrFailAsync(this IUserRepository repository, Guid userId)
        {
            var user = await repository.GetAsync(userId);
            if (user == null)
            {
                throw new ServiceException(ErrorCodes.UserNotFound,
                    $"User with id: '{userId}' was not found.");
            }

            return user;
        }
    }
}
