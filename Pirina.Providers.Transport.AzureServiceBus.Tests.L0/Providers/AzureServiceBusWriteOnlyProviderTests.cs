namespace Pirina.Providers.Transport.AzureServiceBus.Tests.L0.Providers
{
	[TestFixture]
    public class AzureServiceBusWriteOnlyProviderTests
    {
        private const string QueueName = "QueueName";
        private const string ConnectionString = "ConnectionString";

        private Mock<IDependencyResolver> _mockResolver;
        private Mock<IConnectionStringProvider<string>> _mockConnectionStringProvider;

        private Mock<ITransport> _mockTransport;
        private Mock<ITransportManager> _mockTransportManager;
        private Mock<ITransportDispatcher> _mockTransportDispatcher;

        [SetUp]
        public void Init()
        {
            _mockResolver = new Mock<IDependencyResolver>();
            _mockConnectionStringProvider = new Mock<IConnectionStringProvider<string>>();

            _mockTransport = new Mock<ITransport>();
            _mockTransportManager = new Mock<ITransportManager>();
            _mockTransportDispatcher = new Mock<ITransportDispatcher>();

            _mockConnectionStringProvider.Setup(x => x.GetConnectionString()).Returns(ConnectionString);

            _mockResolver.Setup(x => x.Resolve<ITransport>()).Returns(_mockTransport.Object);
            _mockResolver.Setup(x => x.Resolve<ITransportManager>()).Returns(_mockTransportManager.Object);
            _mockResolver.Setup(x => x.Resolve<ITransportDispatcher>()).Returns(_mockTransportDispatcher.Object);
        }

        [TestFixture]
        public class GetDispatcherMethod : AzureServiceBusWriteOnlyProviderTests
        {
            [Test]
            public async Task First_Time_Get_Dispatcher_Is_Called_For_A_Queue_Name_Then_Setup_Will_Be_Called()
            {
                //Arrange
                var writeOnlyProvider = new AzureServiceBusWriteOnlyProvider(_mockConnectionStringProvider.Object, _mockResolver.Object);

                //Act
                var transport = await writeOnlyProvider.GetDispatcher(QueueName);

                //Assert
                _mockResolver.Verify(x => x.Resolve<ITransportDispatcher>(), Times.Once);
                Assert.That(transport, Is.Not.Null);
            }

            [Test]
            public async Task Calling_Get_Dispatcher_Multiple_Times_Will_Only_Resolve_The_Dispatcher_Once()
            {
                //Arrange
                var writeOnlyProvider = new AzureServiceBusWriteOnlyProvider(_mockConnectionStringProvider.Object, _mockResolver.Object);

                //Act
                var transport1 = await writeOnlyProvider.GetDispatcher(QueueName);
                var transport2 = await writeOnlyProvider.GetDispatcher(QueueName);

                //Assert
                _mockResolver.Verify(x => x.Resolve<ITransportDispatcher>(), Times.Once);
                Assert.That(transport1, Is.Not.Null);
                Assert.That(transport2, Is.Not.Null);
            }

            [Test]
            public async Task Multiple_Dispatchers_Can_Be_Registered_To_A_Single_Write_Only_Transport_Provider()
            {
                //Arrange
                var writeOnlyProvider = new AzureServiceBusWriteOnlyProvider(_mockConnectionStringProvider.Object, _mockResolver.Object);

                //Act
                var transport1 = await writeOnlyProvider.GetDispatcher($"{QueueName}1");
                var transport2 = await writeOnlyProvider.GetDispatcher($"{QueueName}2");

                //Assert
                _mockResolver.Verify(x => x.Resolve<ITransportDispatcher>(), Times.Exactly(2));
                Assert.That(transport1, Is.Not.Null);
                Assert.That(transport2, Is.Not.Null);
            }
        }
    }
}
