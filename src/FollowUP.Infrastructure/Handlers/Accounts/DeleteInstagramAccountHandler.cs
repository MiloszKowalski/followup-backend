using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.Commands;
using FollowUP.Infrastructure.Exceptions;
using System.IO;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Handlers.Accounts
{
    public class DeleteInstagramAccountHandler : ICommandHandler<DeleteInstagramAccount>
    {
        private readonly IInstagramAccountRepository _instagramAccountRepository;

        public DeleteInstagramAccountHandler(IInstagramAccountRepository instagramAccountRepository)
        {
            _instagramAccountRepository = instagramAccountRepository;
        }

        public async Task HandleAsync(DeleteInstagramAccount command)
        {
            var account = await _instagramAccountRepository.GetAsync(command.Username);

            if (account == null)
                return;

            if (account.UserId != command.UserId)
                throw new ServiceException(ErrorCodes.UserNotPermitted,
                    "Cannot delete account that doesn't belong to the current user.");

            // Delete account file if exists
            if (File.Exists(account.FilePath))
            {
                File.Delete(account.FilePath);
            }

            await _instagramAccountRepository.RemoveAsync(account.Id);
        }
    }
}
