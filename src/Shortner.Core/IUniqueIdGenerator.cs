using System.Threading.Tasks;

namespace Shortner.Core
{
    public interface IUniqueIdGenerator
    {
        Task<long> GetNext();
    }
}