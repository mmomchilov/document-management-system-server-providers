using Pirina.Common.Initialisation;
using Pirina.Kernel.DependencyResolver;
using Pirina.Providers.Resilience.Polly;
using Pirina.Providers.Transport.AzureServiceBus.Providers;
using Pirina.Providers.Transport.AzureServiceBus.Transport;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Pirina.Providers.Transport.AzureServiceBus.Initialisation
{
    [ExcludeFromCodeCoverage]
    public class AzureServiceBusTransportProviderInitialiser : Initialiser
    {
        public override byte Order => 0;

        protected override Task InitialiseInternal(IDependencyResolver dependencyResolver)
        {
            dependencyResolver.RegisterType<AzureServiceBusReadOnlyProvider>(Lifetime.Singleton);
            dependencyResolver.RegisterType<AzureServiceBusReadWriteProvider>(Lifetime.Singleton);
            dependencyResolver.RegisterType<AzureServiceBusWriteOnlyProvider>(Lifetime.Singleton);

            dependencyResolver.RegisterType<ServiceBusListener>(Lifetime.Transient);
            dependencyResolver.RegisterType<ServiceBusTransportManager>(Lifetime.Transient);
            dependencyResolver.RegisterType<ServiceBusTransportDispatcher>(Lifetime.Transient);
            dependencyResolver.RegisterType<ServiceBusTransport>(Lifetime.Transient);

            dependencyResolver.RegisterType<PollyCircuitBreaker>(Lifetime.Transient);
            dependencyResolver.RegisterType<PollyCircuitBreakerConfiguration>(Lifetime.Transient);

            return Task.CompletedTask;
        }
    }
}