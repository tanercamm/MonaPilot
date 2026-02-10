using MonaPilot.API.Models;

namespace MonaPilot.API.Services
{
    public interface IEventPublisher
    {
        Task PublishProductRequestAsync(ProductRequestEvent @event);
    }
}
