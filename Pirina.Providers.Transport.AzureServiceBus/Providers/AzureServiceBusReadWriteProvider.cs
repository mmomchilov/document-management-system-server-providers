using System.Collections.Generic;
using System.Threading.Tasks;
using Pirina.Common.Transport.Providers;
using Pirina.Kernel.Data.Connection;
using Pirina.Kernel.DependencyResolver;
using Pirina.Kernel.Transport;

namespace Pirina.Providers.Transport.AzureServiceBus.Providers
{
    public class AzureServiceBusReadWriteProvider : AzureServiceBusTransportProvider, IReadWriteTransportProvider
    {
        private readonly Dictionary<string, ITransportDispatcher> _dispatchers;

        public AzureServiceBusReadWriteProvider(IConnectionStringProvider<string> connectionStringResolver,
            IDependencyResolver resolver)
            : base(connectionStringResolver, resolver)
        {
            _dispatchers = new Dictionary<string, ITransportDispatcher>();
        }

        public override async Task Setup(string queueName)
        {
            await CreateListeningManager(queueName);
            _dispatchers.Add(queueName, Resolver.Resolve<ITransportDispatcher>());
        }

        public async Task<ITransportDispatcher> GetDispatcher(string queueName)
        {
            if (!_dispatchers.ContainsKey(queueName))
                await Setup(queueName);

            return _dispatchers[queueName];
        }
    }
}