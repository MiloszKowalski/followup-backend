using FollowUP.Controllers;
using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.Commands;
using FollowUP.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FollowUP.Api.Controllers
{
    [Authorize]
    public class SandboxController : ApiControllerBase
    {
        private readonly IInstagramAccountRepository _accountRepository;
        private readonly IInstagramApiService _apiService;
        private readonly IScheduleRepository _scheduleRepository;

        public SandboxController(IScheduleRepository scheduleRepository,
            ICommandDispatcher commandDispatcher,
            IInstagramApiService apiService,
            IInstagramAccountRepository accountRepository) : base(commandDispatcher)
        {
            _accountRepository = accountRepository;
            _apiService = apiService;
            _scheduleRepository = scheduleRepository;
        }

        [HttpGet("{accountId}")]
        public async Task<IActionResult> Get(Guid accountId)
        {
            var schedules = await _scheduleRepository.GetExplicitScheduleForToday(accountId);

            return Json(schedules);
        }

        [HttpPost("{accountId}/tag/{tag}/{count}")]
        public async Task<IActionResult> UnfollowUsers(Guid accountId, string tag, int count)
        {
            var account = await _accountRepository.GetAsync(accountId);
            var instaApi = await _apiService.GetInstaApiAsync(account);
            await _apiService.SendColdStartMockupRequestsAsync(instaApi, account);
            await Task.Delay(2137);
            await _apiService.GetHashtagMediaAsync(instaApi, account, tag);
            await Task.Delay(3412);
            await _apiService.LikeHashtagMediaAsync(instaApi, account, tag, count);

            return NoContent();
        }
    }
}
