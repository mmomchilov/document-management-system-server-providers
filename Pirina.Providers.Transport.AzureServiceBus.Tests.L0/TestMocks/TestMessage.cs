namespace Pirina.Providers.Transport.AzureServiceBus.Tests.L0.TestMocks
{
	[Serializable]
    internal class TestMessage : Message
    {
        public TestMessage(Guid id, Guid correlationId) : base(id)
        {
        }
    }
}
