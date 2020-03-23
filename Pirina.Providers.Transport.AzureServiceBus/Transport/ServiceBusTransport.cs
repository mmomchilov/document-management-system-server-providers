using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pirina.Providers.Transport.AzureServiceBus.Transport
{
	[ExcludeFromCodeCoverage]
    public class ServiceBusTransport : ITransport
    {
        private QueueClient _queueClient;

        public ServiceBusTransport(Func<ITransportConfiguration> configuration)
        {
            Configuration = configuration();
        }

        public ITransportConfiguration Configuration { get; }
        public bool IsTransactional { get; }
        public int PendingMessages { get; }

        public Task Initialise(CancellationToken cancellationToken)
        {
            if (_queueClient != null)
                return Task.CompletedTask;

            _queueClient = new QueueClient(Configuration.TransportConnection.ConnectionString,
                Configuration.TransportConnection.QueueName);

            if (Configuration.TransportMode == Mode.SendReceive || Configuration.TransportMode == Mode.ReceiveOnly)
                RegisterOnMessageHandlerAndReceiveMessages();

            return Task.CompletedTask;
        }

        public Task Start(CancellationToken cancellationToken)
        {
            if (_queueClient != null)
                return Task.CompletedTask;

            Initialise(cancellationToken);
            return Task.CompletedTask;
        }

        public async Task Stop(CancellationToken cancellationToken)
        {
            if (_queueClient == null)
                return;

            await _queueClient.CloseAsync();
        }

        public async Task Send(byte[] message, CancellationToken cancellationToken)
        {
            try
            {
                await _queueClient.SendAsync(new Message(message));
            }
            catch (Exception ex)
            {
                throw new MessageSendFailure($"Was unable to send Message to '{_queueClient.QueueName}'", ex);
            }
        }

        public Task Send(byte[] message, ITransaction transaction, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<byte[]>> ReadAllMessages(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task CopyMessages(IEnumerable<byte[]> destination, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private void RegisterOnMessageHandlerAndReceiveMessages()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };
            _queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            try
            {
                foreach (var x in Configuration.Listeners)
                    await x.ReceiveMessage(message.Body, token);

                if (Configuration.Listeners.Any())
                    await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}