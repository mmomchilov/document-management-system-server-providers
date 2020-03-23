using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pirina.Providers.Transport.AzureServiceBus.Transport
{
	public class ServiceBusTransportManager : ITransportManager
    {
        private readonly ITransport _transport;
        private readonly ICircuitBreaker _circuitBreaker;
        private readonly IEventLogger _eventLogger;

        public ServiceBusTransportManager(ITransport transport, ICircuitBreaker circuitBreaker, IEventLogger eventLogger)
        {
            _transport = transport ?? throw new ArgumentNullException(nameof(transport));
            _circuitBreaker = circuitBreaker ?? throw new ArgumentNullException(nameof(circuitBreaker));
            _eventLogger = eventLogger ?? throw new ArgumentNullException(nameof(eventLogger));

            _circuitBreaker.CircuitBreakerOpened += CircuitBreakerOnCircuitBreakerOpened;
            _circuitBreaker.CircuitBreakerReset += CircuitBreakerOnCircuitBreakerReset;

            Start(CancellationToken.None);
        }

        private void CircuitBreakerOnCircuitBreakerOpened(object sender, CircuitBreakerOpenedEventArgs args)
        {
            var queueName = _transport.Configuration.TransportConnection.QueueName;
            _eventLogger.Log(SeverityLevel.Warning, EventId.CircuitBreakerOpened, GetType(), 
                $"Circuit Breaker opened on queue '{queueName}' for duration of {args.BreakDuration} due to {args.TriggerException.Message}");
        }

        private void CircuitBreakerOnCircuitBreakerReset(object sender, EventArgs args)
        {
            var queueName = _transport.Configuration.TransportConnection.QueueName;

            _eventLogger.Log(SeverityLevel.Info, EventId.CircuitBreakerReset, GetType(), $"Circuit Breaker on queue '{queueName}' reset");
        }

        public Task Initialise(CancellationToken cancellationToken)
        {
            _transport.Initialise(cancellationToken);
            return Task.CompletedTask;
        }

        public Task Start(CancellationToken cancellationToken)
        {
            _transport.Start(cancellationToken);
            _eventLogger.Log(SeverityLevel.Info, EventId.Started, GetType(), "ServiceBusTransportManager started"); //TODO: no test for this yet

            return Task.CompletedTask;
        }

        public Task Stop(CancellationToken cancellationToken)
        {
            _transport.Stop(cancellationToken);
            _eventLogger.Log(SeverityLevel.Info, EventId.Stopped, GetType(), "ServiceBusTransportManager stopped"); //TODO: no test for this yet

            return Task.CompletedTask;
        }

        public async Task EnqueueMessage(byte[] message, CancellationToken cancellationToken)
        {
            await _circuitBreaker.ExecuteAsync(
                executeAction: async c => await _transport.Send(message, c),
                failureAction: e => throw new MessageSendFailure("Message queuing failure due to Circuit Breaker", e),
                cancellationToken: cancellationToken);
        }

        public Task RegisterListener(IMessageListener listener)
        {
            _transport.Configuration.Listeners.Add(listener);
            return Task.CompletedTask;
        }
    }
}