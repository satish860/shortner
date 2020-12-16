using System.Threading.Tasks;

namespace Shortner.Core
{
    public interface IUrlRepository
    {
        Task<string> GetUrl(long id);
        Task<bool> SaveUrl(long id, string url);
    }
}