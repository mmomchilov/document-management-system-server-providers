namespace Pirina.Providers.Transport.AzureServiceBus.Tests.L0.Providers
{
	[TestFixture]
    public class AzureServiceBusReadOnlyProviderTests
    {
        private const string QueueName = "QueueName";
        private const string ConnectionString = "ConnectionString";

        private Mock<IDependencyResolver> _mockResolver;
        private Mock<IConnectionStringProvider<string>> _mockConnectionStringProvider;

        private Mock<ITransport> _mockTransport;
        private Mock<ITransportManager> _mockTransportManager;
        private Mock<IMessageListener> _mockMessageListener;

        private Mock<IHandlerResolver> _mockHandlerResolver;
        private Mock<IHandlerInvoker> _mockHandlerInvoker;

        [SetUp]
        public void Init()
        {
            _mockResolver = new Mock<IDependencyResolver>();
            _mockConnectionStringProvider = new Mock<IConnectionStringProvider<string>>();

            _mockTransport = new Mock<ITransport>();
            _mockTransportManager = new Mock<ITransportManager>();
            _mockMessageListener = new Mock<IMessageListener>();

            _mockHandlerResolver = new Mock<IHandlerResolver>();
            _mockHandlerInvoker = new Mock<IHandlerInvoker>();

            _mockConnectionStringProvider.Setup(x => x.GetConnectionString()).Returns(ConnectionString);

            _mockResolver.Setup(x => x.Resolve<ITransport>()).Returns(_mockTransport.Object);
            _mockResolver.Setup(x => x.Resolve<ITransportManager>()).Returns(_mockTransportManager.Object);
            _mockResolver.Setup(x => x.Resolve<IMessageListener>()).Returns(_mockMessageListener.Object);
            
            _mockResolver.Setup(x => x.Resolve<IHandlerResolver>()).Returns(_mockHandlerResolver.Object);
            _mockResolver.Setup(x => x.Resolve<IHandlerInvoker>()).Returns(_mockHandlerInvoker.Object);
        }

        [TestFixture]
        public class SetupMethod : AzureServiceBusReadOnlyProviderTests
        {
            [Test]
            public async Task Listener_Can_Be_Resolved_And_Attached_To_Transport_Manager()
            {
                //Arrange
                var readOnlyProvider = new AzureServiceBusReadOnlyProvider(_mockConnectionStringProvider.Object, _mockResolver.Object);

                //Act
                await readOnlyProvider.Setup(QueueName);

                //Assert
                _mockTransportManager.Verify(x => x.RegisterListener(_mockMessageListener.Object), Times.Once);
            }
        }
    }
}
