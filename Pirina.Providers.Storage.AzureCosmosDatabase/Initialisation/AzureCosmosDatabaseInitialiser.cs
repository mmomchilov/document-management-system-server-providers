using System.Threading.Tasks;
using Pirina.Common.Initialisation;
using Pirina.Kernel.DependencyResolver;
using Pirina.Providers.Storage.AzureCosmosDatabase.Configuration;

namespace Pirina.Providers.Storage.AzureCosmosDatabase.Initialisation
{
    public class AzureCosmosDatabaseInitialiser : Initialiser
    {
        public override byte Order => 0;

        protected override Task InitialiseInternal(IDependencyResolver dependencyResolver)
        {
            dependencyResolver.RegisterType<FileTrustMessageStoreConfiguration>(Lifetime.PerThread);
            dependencyResolver.RegisterType<CosmosDbContext>(Lifetime.PerThread);

            return Task.CompletedTask;
        }
    }
}
