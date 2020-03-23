using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Glasswall.Common.Initialisation;
using Glasswall.Common.Serialisation.JSON.Initialisation;
using Glasswall.Kernel.DependencyResolver;
using Glasswall.Providers.Storage.AzureBlob.Connection;
using Glasswall.Providers.Storage.AzureBlob.Factories;
using Glasswall.Providers.Storage.AzureBlob.Store;

namespace Glasswall.Providers.Storage.AzureBlob.Initialisation
{
    [ExcludeFromCodeCoverage]
    public class BlobStoreInitialiser : Initialiser
    {
        public override byte Order => 0;

        protected override async Task InitialiseInternal(IDependencyResolver dependencyResolver)
        {
            await new JsonSerializerInitialiser().Initialise(dependencyResolver);

            dependencyResolver.RegisterType<BlobConnectionManager>(Lifetime.Transient);
            dependencyResolver.RegisterType<BlobConnection>(Lifetime.Transient);
            dependencyResolver.RegisterType<BlobSizeCalculator>(Lifetime.Transient);
            dependencyResolver.RegisterType<BlobStore>(Lifetime.Transient);
            dependencyResolver.RegisterType<BlobStoreFactory>(Lifetime.Singleton);
        }
    }
}
