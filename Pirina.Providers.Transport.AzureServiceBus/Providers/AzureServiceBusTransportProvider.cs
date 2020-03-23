using System.Threading.Tasks;
using Pirina.Common.Transport;
using Pirina.Kernel.Data.Connection;
using Pirina.Kernel.DependencyResolver;
using Pirina.Kernel.Transport;

namespace Pirina.Providers.Transport.AzureServiceBus.Providers
{
    public abstract class AzureServiceBusTransportProvider
    {
        private readonly IConnectionStringProvider<string> _connectionStringResolver;
        protected readonly IDependencyResolver Resolver;

        protected AzureServiceBusTransportProvider(IConnectionStringProvider<string> connectionStringResolver,
            IDependencyResolver resolver)
        {
            _connectionStringResolver = connectionStringResolver;
            Resolver = resolver;
        }

        public abstract Task Setup(string queueName);

        protected async Task CreateListeningManager(string queueName)
        {
            await CreateManager(queueName, Mode.ReceiveOnly);
            var manager = Resolver.Resolve<ITransportManager>();
            var listener = Resolver.Resolve<IMessageListener>();

            await manager.RegisterListener(listener);
        }

        protected async Task CreateNonListeningManager(string queueName)
        {
            await CreateManager(queueName, Mode.SendOnly);
        }

        private async Task CreateManager(string queueName, Mode transportMode)
        {
            await CreateTransport(queueName, transportMode);
        }

        private Task CreateTransport(string queueName, Mode transportMode)
        {
            var connectionString = _connectionStringResolver.GetConnectionString();
            Resolver.RegisterFactory<ITransportConfiguration>(() => new TransportConfiguration
            {
                TransportConnection = new TransportConnection(connectionString, queueName),
                TransportMode = transportMode
            }, Lifetime.Transient);

            return Task.CompletedTask;
        }
    }
}