using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.Commands;
using FollowUP.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Handlers.Accounts
{
    public class DeleteInstagramAccountHandler : ICommandHandler<DeleteInstagramAccount>
    {
        private readonly IInstagramAccountRepository _instagramAccountRepository;
        private readonly IProxyRepository _proxyRepository;
        private readonly IScheduleRepository _scheduleRepository;

        public DeleteInstagramAccountHandler(IInstagramAccountRepository instagramAccountRepository,
            IProxyRepository proxyRepository, IScheduleRepository scheduleRepository)
        {
            _instagramAccountRepository = instagramAccountRepository;
            _proxyRepository = proxyRepository;
            _scheduleRepository = scheduleRepository;
        }

        public async Task HandleAsync(DeleteInstagramAccount command)
        {
            var account = await _instagramAccountRepository.GetAsync(command.InstagramAccountId);

            if (account == null)
            {
                return;
            }

            if (account.User.Id != command.UserId)
            {
                throw new ServiceException(ErrorCodes.UserNotPermitted,
                    "Cannot delete account that doesn't belong to the current user.");
            }

            // Set instagram proxy free
            if (account.InstagramProxy != null)
            {
                var proxy = await _proxyRepository.GetAsync(account.InstagramProxy.Id);
                proxy.SetIsTaken(false);
                await _proxyRepository.UpdateAsync(proxy);
            }
            
            // Delete ScheduleGroups
            IEnumerable<Guid> scheduleGroupsIds = account.ScheduleGroups?.Select(x => x.Id);
            await _scheduleRepository.RemoveMultipleScheduleGroupsAsync(scheduleGroupsIds);

            // Delete SingleScheduleDays
            IEnumerable<Guid> singleScheduleDaysIds = account.SingleScheduleDays?.Select(x => x.Id);
            await _scheduleRepository.RemoveMultipleSingleScheduleDaysAsync(singleScheduleDaysIds);

            // Delete account file if exists
            if (File.Exists(account.FilePath))
            {
                File.Delete(account.FilePath);
            }

            await _instagramAccountRepository.RemoveAsync(account.Id);
        }
    }
}
