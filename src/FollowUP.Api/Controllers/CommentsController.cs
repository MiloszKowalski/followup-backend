using FollowUP.Controllers;
using FollowUP.Infrastructure.Commands;
using FollowUP.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FollowUP.Api.Controllers
{
    public class CommentsController : ApiControllerBase
    {
        private readonly ICommentService _commentsService;

        public CommentsController(ICommentService commentsService,
            ICommandDispatcher commandDispatcher) : base(commandDispatcher)
        {
            _commentsService = commentsService;
        }

        [Authorize]
        [HttpGet("{accountId}")]
        public async Task<IActionResult> Get(Guid accountId)
        {
            var comments = await _commentsService.GetAllRecentByAccountId(accountId);

            return Json(comments);
        }

        [Authorize]
        [HttpPost("{accountId}")]
        public async Task<IActionResult> Post(Guid accountId)
        {
            var succeded = await _commentsService.UpdateAllRecentByAccountId(accountId);

            return Json(succeded);
        }
    }
}
