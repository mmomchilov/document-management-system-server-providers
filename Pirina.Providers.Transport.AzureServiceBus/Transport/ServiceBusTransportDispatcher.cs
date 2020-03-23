using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Pirina.Providers.Transport.AzureServiceBus.Transport
{
	public class ServiceBusTransportDispatcher : ITransportDispatcher
    {
        private readonly ISerialiser _serialiser;

        public ServiceBusTransportDispatcher(ITransportManager transportManager, ISerialiser serialiser)
        {
            TransportManager = transportManager;
            _serialiser = serialiser;
        }

        public ITransportManager TransportManager { get; }

        public async Task SendMessage<TMessage>(TMessage message, CancellationToken cancellationToken)
            where TMessage : Message
        {
            //TODO: check that TMessage is serizable else throw, or at least log

            using (var ms = new MemoryStream())
            {
                var stream = await _serialiser.Serialise(message);
                stream.Position = 0;
                stream.CopyTo(ms);

                await TransportManager.EnqueueMessage(ms.ToArray(), cancellationToken);
            }
        }
    }
}