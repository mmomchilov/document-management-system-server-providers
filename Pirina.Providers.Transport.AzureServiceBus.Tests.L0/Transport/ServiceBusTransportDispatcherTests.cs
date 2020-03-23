namespace Pirina.Providers.Transport.AzureServiceBus.Tests.L0.Transport
{
	[TestFixture]
    [Category("L0")]
    public class ServiceBusTransportDispatcherTests
    {
        private Mock<ISerialiser> _mockSerialiser;
        private Mock<ITransportManager> _mockTransportManager;

        [SetUp]
        public void Init()
        {
            _mockSerialiser = new Mock<ISerialiser>();
            _mockTransportManager = new Mock<ITransportManager>();
        }

        public class SendMessageMethod : ServiceBusTransportDispatcherTests
        {
            [Test]
            public async Task Transport_Dispatcher_Can_Send_Message()
            {
                //Arrange
                var mockBytes = new byte[] { 72, 101, 108, 108, 111, 32, 87, 111, 114, 108, 100, 33 };
                var mockStream = new MemoryStream(mockBytes);
                var mockMessage = new TestMessage(Guid.Empty, Guid.Empty);
                _mockSerialiser.
                    Setup(x => x.Serialise(It.IsAny<object>())).
                    Returns(Task.FromResult((Stream) mockStream));
                _mockTransportManager.Setup(x => x.EnqueueMessage(It.IsAny<byte[]>(), CancellationToken.None)).Returns(Task.FromResult(true));
                var dispatcher =
                    new ServiceBusTransportDispatcher(_mockTransportManager.Object, _mockSerialiser.Object);

                //Act
                await dispatcher.SendMessage(mockMessage, CancellationToken.None);

                //Assert
                _mockTransportManager.Verify(x => x.EnqueueMessage(It.IsAny<byte[]>(), CancellationToken.None), Times.Once);
            }

            [Test]
            public void A_Serialisation_Failure_Will_Bubble_Up_To_Send_Message()
            {
                //Arrange
                const string errorMessage = "I am an Error Message!";
                var mockMessage = new TestMessage(Guid.Empty, Guid.Empty);
                _mockSerialiser.
                    Setup(x => x.Serialise(It.IsAny<object>())).
                    Throws(new Exception(errorMessage));
                var dispatcher =
                    new ServiceBusTransportDispatcher(_mockTransportManager.Object, _mockSerialiser.Object);

                async Task Delegate()
                {
                    await dispatcher.SendMessage(mockMessage, CancellationToken.None);
                }

                //Assert
                Assert.That(Delegate, Throws.Exception.Message.EqualTo(errorMessage));
            }

            [Test]
            public void If_Message_Fails_To_Send_Then_A_Message_Send_Failure_Exception_Is_Thrown()
            {
                //Arrange
                var mockBytes = new byte[] { 72, 101, 108, 108, 111, 32, 87, 111, 114, 108, 100, 33 };
                var mockStream = new MemoryStream(mockBytes);
                var mockMessage = new TestMessage(Guid.Empty, Guid.Empty);
                _mockSerialiser.
                    Setup(x => x.Serialise(It.IsAny<object>())).
                    Returns(Task.FromResult((Stream)mockStream));
                _mockTransportManager.
                    Setup(x => x.EnqueueMessage(It.IsAny<byte[]>(), CancellationToken.None)).
                    Returns(Task.FromResult(false)).
                    Callback(() => throw new MessageSendFailure());
                var dispatcher =
                    new ServiceBusTransportDispatcher(_mockTransportManager.Object, _mockSerialiser.Object);

                //Act
                async Task Delegate()
                {
                    await dispatcher.SendMessage(mockMessage, CancellationToken.None);
                }

                //Assert
                Assert.That(Delegate, Throws.TypeOf(typeof(MessageSendFailure)));
            }
        }
    }
}
