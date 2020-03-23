namespace Pirina.Providers.Transport.AzureServiceBus.Logging
{
    public enum EventId
    {
        Started = Common.Logging.ComponentId.GlasswallProvidersTransportAzureServiceBus,
        Stopped,
        CircuitBreakerOpened,
        CircuitBreakerReset
    }
}
