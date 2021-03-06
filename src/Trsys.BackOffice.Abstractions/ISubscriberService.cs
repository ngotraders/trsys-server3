using System.Threading;
using System.Threading.Tasks;

namespace Trsys.BackOffice.Abstractions
{
    public interface ISubscriberService
    {
        Task<PagedResult<SubscriberDto>> SearchAsync(int page, int perPage, CancellationToken cancellationToken);
        Task<SubscriberDto> FindByIdAsync(string subscriberId, CancellationToken cancellationToken);
        Task<string> CreateAsync(string name, string description, CancellationToken cancellationToken);
        Task UpdateNameAsync(string subscriberId, string name, CancellationToken cancellationToken);
        Task UpdateDescriptionAsync(string subscriberId, string description, CancellationToken cancellationToken);
        Task DeleteAsync(string subscriberId, CancellationToken cancellationToken);
    }
}