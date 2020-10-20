using AutoMapper;
using FollowUP.Infrastructure.Commands;
using FollowUP.Infrastructure.Commands.Accounts;
using FollowUP.Infrastructure.DTO;
using FollowUP.Infrastructure.Services;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Handlers
{
    class UpdateAccountSettingsHandler : ICommandHandler<UpdateAccountSettings>
    {
        private readonly IMapper _mapper;
        private readonly IAccountSettingsService _accountSettingsService;

        public UpdateAccountSettingsHandler(IMapper mapper,
            IAccountSettingsService accountSettingsService)
        {
            _mapper = mapper;
            _accountSettingsService = accountSettingsService;
        }

        public async Task HandleAsync(UpdateAccountSettings command)
        {
            var settings = _mapper.Map<AccountSettingsDto>(command);
            await _accountSettingsService.UpdateAccountSettingsAsync(command.InstagramAccountId,
                command.UserId, settings);
        }
    }
}
