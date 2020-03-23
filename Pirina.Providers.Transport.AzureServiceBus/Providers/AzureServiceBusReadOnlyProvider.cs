using System.Threading.Tasks;
using Pirina.Common.Transport.Providers;
using Pirina.Kernel.Data.Connection;
using Pirina.Kernel.DependencyResolver;

namespace Pirina.Providers.Transport.AzureServiceBus.Providers
{
    public class AzureServiceBusReadOnlyProvider : AzureServiceBusTransportProvider, IReadOnlyTransportProvider
    {
        public AzureServiceBusReadOnlyProvider(IConnectionStringProvider<string> connectionStringResolver,
            IDependencyResolver resolver)
            : base(connectionStringResolver, resolver)
        {
        }

        public override async Task Setup(string queueName)
        {
            await CreateListeningManager(queueName);
        }
    }
}