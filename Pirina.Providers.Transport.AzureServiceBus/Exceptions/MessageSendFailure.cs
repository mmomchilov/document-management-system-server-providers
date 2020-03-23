using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Pirina.Providers.Transport.AzureServiceBus.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class MessageSendFailure : Exception
    {
        public MessageSendFailure()
        {
        }

        public MessageSendFailure(string message) : base(message)
        {
        }

        public MessageSendFailure(string message, Exception inner) : base(message, inner)
        {
        }

        protected MessageSendFailure(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
