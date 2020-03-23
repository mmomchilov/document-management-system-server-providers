namespace Pirina.Providers.Transport.AzureServiceBus.Tests.L0.TestMocks
{
	internal class TestMessageHandler : BaseMessageHandler<TestMessage>
    {
        public TestMessageHandler(IEventLogger logger) : base(logger)
        {
        }

        protected override Task InvokeInternal(TestMessage message, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
