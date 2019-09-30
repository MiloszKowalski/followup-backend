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
            var comments = await _commentsService.GetAllByAccountId(accountId);

            return Json(comments);
        }

        [Authorize]
        [HttpGet("{accountId}/{page}/{pageSize}")]
        public async Task<IActionResult> GetPaginated(Guid accountId, int page, int pageSize)
        {
            var comments = await _commentsService.GetByAccountId(accountId, page, pageSize);

            return Json(comments);
        }

        [Authorize]
        [HttpGet("{accountId}/count")]
        public async Task<IActionResult> GetCommentsCount(Guid accountId)
        {
            var count = await _commentsService.GetCount(accountId);

            return Json(count);
        }

        [Authorize]
        [HttpPost("{accountId}")]
        public async Task<IActionResult> Post(Guid accountId)
        {
            await _commentsService.UpdateAllByAccountId(accountId);

            return Ok();
        }
    }
}
