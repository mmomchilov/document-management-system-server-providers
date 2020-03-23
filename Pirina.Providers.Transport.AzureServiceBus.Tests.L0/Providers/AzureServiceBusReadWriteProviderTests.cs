namespace Glasswall.Providers.Transport.AzureServiceBus.Tests.L0.Providers
{
	[TestFixture]
    public class AzureServiceBusReadWriteProviderTests
    {
        private const string QueueName = "QueueName";
        private const string ConnectionString = "ConnectionString";

        private Mock<IDependencyResolver> _mockResolver;
        private Mock<IConnectionStringProvider<string>> _mockConnectionStringProvider;

        private Mock<ITransport> _mockTransport;
        private Mock<ITransportManager> _mockTransportManager;
        private Mock<IMessageListener> _mockMessageListener;
        private Mock<ITransportDispatcher> _mockTransportDispatcher;

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
            _mockTransportDispatcher = new Mock<ITransportDispatcher>();

            _mockHandlerResolver = new Mock<IHandlerResolver>();
            _mockHandlerInvoker = new Mock<IHandlerInvoker>();

            _mockConnectionStringProvider.Setup(x => x.GetConnectionString()).Returns(ConnectionString);

            _mockResolver.Setup(x => x.Resolve<ITransport>()).Returns(_mockTransport.Object);
            _mockResolver.Setup(x => x.Resolve<ITransportManager>()).Returns(_mockTransportManager.Object);
            _mockResolver.Setup(x => x.Resolve<IMessageListener>()).Returns(_mockMessageListener.Object);
            _mockResolver.Setup(x => x.Resolve<ITransportDispatcher>()).Returns(_mockTransportDispatcher.Object);

            _mockResolver.Setup(x => x.Resolve<IHandlerResolver>()).Returns(_mockHandlerResolver.Object);
            _mockResolver.Setup(x => x.Resolve<IHandlerInvoker>()).Returns(_mockHandlerInvoker.Object);
        }

        [TestFixture]
        public class SetupMethod : AzureServiceBusReadWriteProviderTests
        {
            [Test]
            public async Task Listener_Can_Be_Resolved_And_Attached_To_Transport_Manager()
            {
                //Arrange
                var readWriteProvider = new AzureServiceBusReadWriteProvider(_mockConnectionStringProvider.Object, _mockResolver.Object);

                //Act
                await readWriteProvider.Setup(QueueName);

                //Assert
                _mockTransportManager.Verify(x => x.RegisterListener(_mockMessageListener.Object), Times.Once);
            }
        }

        [TestFixture]
        public class GetDispatcherMethod : AzureServiceBusReadWriteProviderTests
        {
            [Test]
            public async Task First_Time_Get_Dispatcher_Is_Called_For_A_Queue_Name_Then_Setup_Will_Be_Called()
            {
                //Arrange
                var readWriteProvider = new AzureServiceBusReadWriteProvider(_mockConnectionStringProvider.Object, _mockResolver.Object);

                //Act
                var transport = await readWriteProvider.GetDispatcher(QueueName);

                //Assert
                _mockResolver.Verify(x => x.Resolve<ITransportDispatcher>(), Times.Once);
                Assert.That(transport, Is.Not.Null);
            }

            [Test]
            public async Task Calling_Get_Dispatcher_Multiple_Times_Will_Only_Resolve_The_Dispatcher_Once()
            {
                //Arrange
                var readWriteProvider = new AzureServiceBusReadWriteProvider(_mockConnectionStringProvider.Object, _mockResolver.Object);

                //Act
                var transport1 = await readWriteProvider.GetDispatcher(QueueName);
                var transport2 = await readWriteProvider.GetDispatcher(QueueName);

                //Assert
                _mockResolver.Verify(x => x.Resolve<ITransportDispatcher>(), Times.Once);
                Assert.That(transport1, Is.Not.Null);
                Assert.That(transport2, Is.Not.Null);
            }

            [Test]
            public async Task Multiple_Dispatchers_Can_Be_Registered_To_A_Single_Read_Write_Transport_Provider()
            {
                //Arrange
                var readWriteProvider = new AzureServiceBusReadWriteProvider(_mockConnectionStringProvider.Object, _mockResolver.Object);

                //Act
                var transport1 = await readWriteProvider.GetDispatcher($"{QueueName}1");
                var transport2 = await readWriteProvider.GetDispatcher($"{QueueName}2");

                //Assert
                _mockResolver.Verify(x => x.Resolve<ITransportDispatcher>(), Times.Exactly(2));
                Assert.That(transport1, Is.Not.Null);
                Assert.That(transport2, Is.Not.Null);
            }
        }
    }
}
