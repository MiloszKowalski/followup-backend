using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.Commands;
using FollowUP.Infrastructure.Exceptions;
using FollowUP.Infrastructure.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Handlers.Accounts
{
    public class DeleteInstagramAccountHandler : ICommandHandler<DeleteInstagramAccount>
    {
        private readonly IInstagramAccountService _instagramAccountService;
        private readonly IInstagramAccountRepository _instagramAccountRepository;

        public DeleteInstagramAccountHandler(IInstagramAccountService instagramAccountService,
                                        IInstagramAccountRepository instagramAccountRepository)
        {
            _instagramAccountService = instagramAccountService;
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

            // Get appropriate directories of the folder and file
            var fullPath = account.FilePath.Split(@"\");
            var directory = $@"{fullPath[0]}\{fullPath[1]}";

            // Delete account file if exists
            if (File.Exists(account.FilePath))
            {
                File.Delete(account.FilePath);
            }

            await _instagramAccountRepository.RemoveAsync(account.Id);
        }
    }
}
