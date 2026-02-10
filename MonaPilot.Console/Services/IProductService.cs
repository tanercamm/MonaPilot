using System.Threading.Tasks;
using MonaPilot.Console.Models;

namespace MonaPilot.Console.Services
{
    public interface IProductService
    {
        Task ProcessProductRequestAsync(ProductRequestEvent @event);
    }
}
