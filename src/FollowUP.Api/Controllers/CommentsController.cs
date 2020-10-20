using FollowUP.Controllers;
using FollowUP.Infrastructure.Commands;
using FollowUP.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FollowUP.Api.Controllers
{
    [Authorize]
    public class CommentsController : ApiControllerBase
    {
        private readonly ICommentService _commentsService;

        public CommentsController(ICommentService commentsService,
            ICommandDispatcher commandDispatcher) : base(commandDispatcher)
        {
            _commentsService = commentsService;
        }

        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetAllCommentsByAccountId(Guid accountId)
        {
            var comments = await _commentsService.GetAllByAccountIdAsync(accountId);

            return Json(comments);
        }

        [HttpGet("{accountId}/{page}/{pageSize}")]
        public async Task<IActionResult> GetPaginatedCommentsByAccountId(Guid accountId,
            int page, int pageSize)
        {
            var comments = await _commentsService.GetByAccountIdAsync(accountId, page, pageSize);

            return Json(comments);
        }

        [HttpGet("{accountId}/count")]
        public async Task<IActionResult> GetCommentsCount(Guid accountId)
        {
            var count = await _commentsService.GetCountAsync(accountId);

            return Json(count);
        }

        [HttpPost("{accountId}")]
        public async Task<IActionResult> Post(Guid accountId)
        {
            await _commentsService.UpdateAllByAccountIdAsync(accountId);

            return Ok();
        }
    }
}
