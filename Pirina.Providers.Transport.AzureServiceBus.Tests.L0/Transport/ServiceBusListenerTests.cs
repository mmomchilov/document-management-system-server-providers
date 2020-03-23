namespace Pirina.Providers.Transport.AzureServiceBus.Tests.L0.Transport
{
	[TestFixture]
    [Category("L0")]
    public class ServiceBusListenerTests
    {
        private Mock<ISerialiser> _mockSerialiser;
        private Mock<IHandlerResolver> _mockHandlerResolver;
        private Mock<IHandlerInvoker> _mockHandlerInvoker;
        private Mock<IEventLogger> _mockEventLogger;

        [SetUp]
        public void Init()
        {
            _mockSerialiser = new Mock<ISerialiser>();
            _mockHandlerResolver = new Mock<IHandlerResolver>();
            _mockHandlerInvoker = new Mock<IHandlerInvoker>();
            _mockEventLogger = new Mock<IEventLogger>();
        }

        [TestFixture]
        public class StartMethod : ServiceBusListenerTests
        {
            [Test]
            public async Task Start_Will_Return_True()
            {
                //Arrange 
                var listener = new ServiceBusListener(_mockSerialiser.Object, _mockHandlerResolver.Object,
                    _mockHandlerInvoker.Object);

                //Act
                var result = await listener.Start();

                //Assert
                Assert.That(result, Is.True);
            }
        }

        [TestFixture]
        public class StopMethod : ServiceBusListenerTests
        {
            [Test]
            public async Task Stop_Will_Return_True()
            {
                //Arrange 
                var listener = new ServiceBusListener(_mockSerialiser.Object, _mockHandlerResolver.Object,
                    _mockHandlerInvoker.Object);

                //Act
                var result = await listener.Stop();

                //Assert
                Assert.That(result, Is.True);
            }
        }

        [TestFixture]
        public class AttachToMethod : ServiceBusListenerTests
        {
            [Test]
            public async Task Listener_Can_Be_Attached_To_An_Active_Transport_Manager()
            {
                //Arrange 
                var transport = new Mock<ITransport>();
                transport.Setup(x => x.Configuration).Returns(new TransportConfiguration());
                var transportManager = new ServiceBusTransportManager(transport.Object, Mock.Of<ICircuitBreaker>(), Mock.Of<IEventLogger>());
                var listener = new ServiceBusListener(_mockSerialiser.Object, _mockHandlerResolver.Object,
                    _mockHandlerInvoker.Object);

                //Act
                var result = await listener.AttachTo(transportManager);

                //Assert
                Assert.That(result, Is.True);
                Assert.That(transport.Object.Configuration.Listeners, Has.Exactly(1).EqualTo(listener));
            }

            [Test]
            public async Task Failing_To_Attach_To_Transport_Manager_Will_Return_False()
            {
                //Arrange 
                var transport = new Mock<ITransport>();
                var transportManager = new ServiceBusTransportManager(transport.Object, Mock.Of<ICircuitBreaker>(), Mock.Of<IEventLogger>());
                var listener = new ServiceBusListener(_mockSerialiser.Object, _mockHandlerResolver.Object,
                    _mockHandlerInvoker.Object);

                //Act
                var result = await listener.AttachTo(transportManager);

                //Assert
                Assert.That(result, Is.False);
            }
        }

        [TestFixture]
        public class ReceiveMessageMethod : ServiceBusListenerTests
        {
            [Test]
            public async Task When_Message_Is_Received_The_Correct_Handler_Is_Invoked()
            {
                //Arrange
                var mockBytes = new byte[] {72, 101, 108, 108, 111, 32, 87, 111, 114, 108, 100, 33};
                var mockMessage = new TestMessage(Guid.Empty, Guid.Empty);
                var mockHandler = new TestMessageHandler(_mockEventLogger.Object);
                _mockSerialiser.
                    Setup(x => x.Deserialise(It.IsAny<MemoryStream>())).
                    Returns(Task.FromResult((object) mockMessage));
                _mockHandlerResolver.
                    Setup(x => x.ResolveAllHandlersFor(typeof(TestMessage))).
                    Returns(new[] { mockHandler });
                var listener = new ServiceBusListener(_mockSerialiser.Object, _mockHandlerResolver.Object,
                    _mockHandlerInvoker.Object);

                //Act
                await listener.ReceiveMessage(mockBytes, CancellationToken.None);


                //Assert
                _mockHandlerInvoker.Verify(x => x.InvokeHandlers(new [] {mockHandler}, mockMessage, CancellationToken.None), Times.Once);
            }

            [Test]
            public async Task If_Deserialiser_Fails_Then_The_Exception_Will_Bubble_Up_Throw_The_Receive_Message()
            {
                //Arrange
                const string errorMessage = "Hello From The Test World";
                var mockBytes = new byte[] { 72, 101, 108, 108, 111, 32, 87, 111, 114, 108, 100, 33 };
                _mockSerialiser.
                    Setup(x => x.Deserialise(It.IsAny<MemoryStream>())).
                    Throws(new Exception(errorMessage));
                var listener = new ServiceBusListener(_mockSerialiser.Object, _mockHandlerResolver.Object,
                    _mockHandlerInvoker.Object);

                try
                {
                    //Act
                    await listener.ReceiveMessage(mockBytes, CancellationToken.None);
                }
                catch (Exception e)
                {
                    //Assert
                    Assert.That(e.Message, Is.EqualTo(errorMessage));
                }
            }

            [Test]
            public void If_Empty_Byte_Array_Is_Received_Then_A_Null_Reference_Exception_Should_Be_Thrown()
            {
                //Arrange
                var mockBytes = new byte[] { };
                var listener = new ServiceBusListener(_mockSerialiser.Object, _mockHandlerResolver.Object,
                    _mockHandlerInvoker.Object);

                //Act
                async Task Delegate()
                {
                    await listener.ReceiveMessage(mockBytes, CancellationToken.None);
                }

                //Assert
                Assert.That(Delegate, Throws.TypeOf<NullReferenceException>());
            }
        }

        [TestFixture]
        public class ReceiveMessageTransactionalMethod : ServiceBusListenerTests
        {
            [Test]
            public void Receive_Message_Transactional_Will_Throw_A_Not_Implemented_Exception()
            {
                //Arrange
                var mockBytes = new byte[] {72, 101, 108, 108, 111, 32, 87, 111, 114, 108, 100, 33};
                var mockTransaction = new Mock<ITransaction>();
                var listener = new ServiceBusListener(_mockSerialiser.Object, _mockHandlerResolver.Object,
                    _mockHandlerInvoker.Object);

                //Act
                async Task Delegate()
                {
                    await listener.ReceiveMessageTransactional(mockBytes, mockTransaction.Object, CancellationToken.None);
                }

                //Assert
                Assert.That(Delegate, Throws.TypeOf<NotImplementedException>());
            }
        }
    }
}