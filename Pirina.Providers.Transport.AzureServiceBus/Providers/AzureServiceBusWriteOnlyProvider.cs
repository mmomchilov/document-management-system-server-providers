using System.Collections.Generic;
using System.Threading.Tasks;
using Pirina.Common.Transport.Providers;
using Pirina.Kernel.Data.Connection;
using Pirina.Kernel.DependencyResolver;
using Pirina.Kernel.Transport;

namespace Pirina.Providers.Transport.AzureServiceBus.Providers
{
    public class AzureServiceBusWriteOnlyProvider : AzureServiceBusTransportProvider, IWriteOnlyTransportProvider
    {
        private readonly Dictionary<string, ITransportDispatcher> _dispatchers;

        public AzureServiceBusWriteOnlyProvider(IConnectionStringProvider<string> connectionStringResolver,
            IDependencyResolver resolver)
            : base(connectionStringResolver, resolver)
        {
            _dispatchers = new Dictionary<string, ITransportDispatcher>();
        }

        public async Task<ITransportDispatcher> GetDispatcher(string queueName)
        {
            if (!_dispatchers.ContainsKey(queueName))
                await Setup(queueName);

            return _dispatchers[queueName];
        }

        public override async Task Setup(string queueName)
        {
            await CreateNonListeningManager(queueName);
            _dispatchers.Add(queueName, Resolver.Resolve<ITransportDispatcher>());
        }
    }
}