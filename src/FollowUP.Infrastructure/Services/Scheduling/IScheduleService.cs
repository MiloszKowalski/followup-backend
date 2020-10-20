using System;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    public interface IScheduleService : IService
    {
        Task SchedulePromotionQueueForTodayAsync(Guid accountId);
    }
}
