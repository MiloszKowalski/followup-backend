using FollowUP.Controllers;
using FollowUP.Infrastructure.Commands;
using FollowUP.Infrastructure.Commands.Accounts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FollowUP.Api.Controllers
{
    public class RegisterController : ApiControllerBase
    {
        public RegisterController(ICommandDispatcher commandDispatcher)
            : base(commandDispatcher)
        {
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Register command)
        {
            await DispatchAsync(command);

            return Created($"users/{command.Email}", null);
        }
    }
}
