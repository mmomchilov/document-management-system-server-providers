using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Pirina.Providers.Transport.AzureServiceBus.Transport
{
	public class ServiceBusListener : IMessageListener
    {
        private readonly IHandlerInvoker _handlerInvoker;
        private readonly IHandlerResolver _handlerResolver;
        private readonly ISerialiser _serialiser;

        public ServiceBusListener(ISerialiser serialiser, IHandlerResolver handlerResolver,
            IHandlerInvoker handlerInvoker)
        {
            _serialiser = serialiser;
            _handlerInvoker = handlerInvoker;
            _handlerResolver = handlerResolver;
        }

        public Task<bool> Start()
        {
            return Task.FromResult(true);
        }

        public Task<bool> Stop()
        {
            return Task.FromResult(true);
        }

        public async Task<bool> AttachTo(ITransportManager transportManager)
        {
            try
            {
                await transportManager.RegisterListener(this);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task ReceiveMessage(byte[] message, CancellationToken cancellationToken)
        {
            object obj;

            using (var memoryStream = new MemoryStream(message))
            {
                obj = await _serialiser.Deserialise(memoryStream);
            }

            var handlers = _handlerResolver.ResolveAllHandlersFor(obj.GetType());
            await _handlerInvoker.InvokeHandlers(handlers, obj, cancellationToken);
        }

        public Task ReceiveMessageTransactional(byte[] message, ITransaction transaction, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}