using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using AcmStatisticsBackend.Crawlers;

namespace AcmStatisticsBackend.Authorization.Users
{
    public class UserDeletingEventHandler : IAsyncEventHandler<EntityDeletingEventData<User>>, ITransientDependency
    {
        private readonly IRepository<DefaultQuery, long> _defaultQueryRepository;
        private readonly IRepository<QueryHistory, long> _acHistoryRepository;

        public UserDeletingEventHandler(IRepository<DefaultQuery, long> defaultQueryRepository,
            IRepository<QueryHistory, long> acHistoryRepository)
        {
            _defaultQueryRepository = defaultQueryRepository;
            _acHistoryRepository = acHistoryRepository;
        }

        public async Task HandleEventAsync(EntityDeletingEventData<User> eventData)
        {
            await _defaultQueryRepository.HardDeleteAsync(
                e => e.UserId == eventData.Entity.Id);
            await _acHistoryRepository.DeleteAsync(
                e => e.UserId == eventData.Entity.Id);
        }
    }
}
