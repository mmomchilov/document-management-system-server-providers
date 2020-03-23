using System.Threading.Tasks;

namespace Glasswall.Providers.Storage.AzureBlob.Store
{
    public interface IBlobSizeCalculator
    {
        Task<int> GetBlockSize(long streamLength);
    }
}