namespace Pirina.Providers.Transport.AzureServiceBus.Tests.L0.Transport
{
	[TestFixture]
    [Category("L0")]
    public class ServiceBusTransportManagerTests
    {
        private Mock<ITransport> _mockTransport;
        private Mock<ICircuitBreaker> _mockCircuitBreaker;
        private Mock<IEventLogger> _mockEventLogger;

        [SetUp]
        public void Init()
        {
            _mockTransport = new Mock<ITransport>();
            _mockCircuitBreaker = new Mock<ICircuitBreaker>();
            _mockEventLogger = new Mock<IEventLogger>();
        }

        [TestFixture]
        public class Constructor : ServiceBusTransportManagerTests
        {
            [Test]
            public void Throws_ArgumentNullException_If_ITransport_Is_Null()
            {
                // Act
                var thrownException = Assert.Throws<ArgumentNullException>(() =>
                    new ServiceBusTransportManager(null, Mock.Of<ICircuitBreaker>(), Mock.Of<IEventLogger>())
                );

                // Assert
                Assert.That(thrownException.ParamName, Is.EqualTo("transport"));
            }
            [Test]
            public void Throws_ArgumentNullException_If_ICircuitBreaker_Is_Null()
            {
                // Act
                var thrownException = Assert.Throws<ArgumentNullException>(() =>
                    new ServiceBusTransportManager(Mock.Of<ITransport>(), null, Mock.Of<IEventLogger>())
                );

                // Assert
                Assert.That(thrownException.ParamName, Is.EqualTo("circuitBreaker"));
            }
            [Test]
            public void Throws_ArgumentNullException_If_IEventLogger_Is_Null()
            {
                // Act
                var thrownException = Assert.Throws<ArgumentNullException>(() =>
                    new ServiceBusTransportManager(Mock.Of<ITransport>(), Mock.Of<ICircuitBreaker>(), null)
                );

                // Assert
                Assert.That(thrownException.ParamName, Is.EqualTo("eventLogger"));
            }
            [Test]
            public void Valid_Arguments()
            {
                // Act
                var serviceBusTransportManager = new ServiceBusTransportManager(Mock.Of<ITransport>(), Mock.Of<ICircuitBreaker>(), Mock.Of<IEventLogger>());

                // Assert
                Assert.That(serviceBusTransportManager, Is.Not.Null);
            }
        }

        [TestFixture]
        public class InitialiseMethod : ServiceBusTransportManagerTests
        {
            [Test]
            public async Task Initialise_Calls_Initialise_On_The_Transport()
            {
                //Arrange
                var manager = new ServiceBusTransportManager(_mockTransport.Object, Mock.Of<ICircuitBreaker>(), Mock.Of<IEventLogger>());

                //Act
                await manager.Initialise(CancellationToken.None);

                //Assert
                _mockTransport.Verify(x => x.Initialise(CancellationToken.None), Times.Once);
            }
        }

        [TestFixture]
        public class StartMethod : ServiceBusTransportManagerTests
        {
            [Test]
            public async Task Start_Calls_Start_On_The_Transport()
            {
                //Arrange
                var manager = new ServiceBusTransportManager(_mockTransport.Object, Mock.Of<ICircuitBreaker>(), Mock.Of<IEventLogger>());

                //Act
                await manager.Start(CancellationToken.None);

                //Assert
                //This is expect as twice because it was called once from the constructor
                _mockTransport.Verify(x => x.Start(CancellationToken.None), Times.Exactly(2));
            }
        }

        [TestFixture]
        public class StopMethod : ServiceBusTransportManagerTests
        {
            [Test]
            public async Task Stop_Calls_Stop_On_The_Transport()
            {
                //Arrange
                var manager = new ServiceBusTransportManager(_mockTransport.Object, Mock.Of<ICircuitBreaker>(), Mock.Of<IEventLogger>());

                //Act
                await manager.Stop(CancellationToken.None);

                //Assert
                _mockTransport.Verify(x => x.Stop(CancellationToken.None), Times.Once);
            }
        }

        [TestFixture]
        public class EnqueueMessageMethod : ServiceBusTransportManagerTests
        {
            [Test]
            public async Task Enqueue_Message_Passes_Message_To_Transport_Successfully()
            {
                //Arrange
                const string expectedMessage = "Test Message";
                var receivedMessage = string.Empty;
                _mockTransport.
                    Setup(m => m.Send(It.IsAny<byte[]>(), It.IsAny<CancellationToken>())).
                    Returns(Task.CompletedTask).
                    Callback((byte[] message, CancellationToken token) => receivedMessage = Encoding.ASCII.GetString(message));

                _mockCircuitBreaker.
                    Setup(m => m.ExecuteAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<Func<Exception, Task>>(), It.IsAny<CancellationToken>())).
                    Returns(Task.CompletedTask).
                    Callback((Func<CancellationToken, Task> executeAction, Func<Exception, Task> failureAction, CancellationToken c) => executeAction(c));

                var manager = new ServiceBusTransportManager(_mockTransport.Object, _mockCircuitBreaker.Object, Mock.Of<IEventLogger>());

                //Act
                await manager.EnqueueMessage(Encoding.ASCII.GetBytes(expectedMessage), CancellationToken.None);

                //Assert
                Assert.That(receivedMessage, Is.EqualTo(expectedMessage));
            }

            [Test]
            public void Enqueue_Message_Throws_Exception_When_Transport_Fails_To_Send_The_Message()
            {
                _mockCircuitBreaker.
                    Setup(m => m.ExecuteAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<Func<Exception, Task>>(), It.IsAny<CancellationToken>())).
                    Returns(Task.CompletedTask).
                    Callback((Func<CancellationToken, Task> executeAction, Func<Exception, Task> failureAction, CancellationToken c) => failureAction(Mock.Of<Exception>()));

                var manager = new ServiceBusTransportManager(_mockTransport.Object, _mockCircuitBreaker.Object, Mock.Of<IEventLogger>());

                async Task TestDelegateAsync() => await manager.EnqueueMessage(It.IsAny<byte[]>(), CancellationToken.None);

                Assert.ThrowsAsync<MessageSendFailure>(TestDelegateAsync);
            }
        }

        [TestFixture]
        public class RegisterListenerMethod : ServiceBusTransportManagerTests
        {
            [Test]
            public async Task A_Single_Listener_Can_Be_Registered_On_The_Transport_Manager()
            {
                //Arrange
                _mockTransport.Setup(x => x.Configuration).Returns(new TransportConfiguration());
                var mockListerner = new Mock<IMessageListener>();
                var manager = new ServiceBusTransportManager(_mockTransport.Object, Mock.Of<ICircuitBreaker>(), Mock.Of<IEventLogger>());

                //Act
                await manager.RegisterListener(mockListerner.Object);

                //Assert
                Assert.That(_mockTransport.Object.Configuration.Listeners, Has.Exactly(1).EqualTo(mockListerner.Object));
            }

            [Test]
            public async Task Multiple_Listener_Can_Be_Registered_On_The_Transport_Manager()
            {
                //Arrange
                _mockTransport.Setup(x => x.Configuration).Returns(new TransportConfiguration());
                var mockListerner1 = new Mock<IMessageListener>();
                var mockListerner2 = new Mock<IMessageListener>();
                var manager = new ServiceBusTransportManager(_mockTransport.Object, Mock.Of<ICircuitBreaker>(), Mock.Of<IEventLogger>());

                //Act
                await manager.RegisterListener(mockListerner1.Object);
                await manager.RegisterListener(mockListerner2.Object);

                //Assert
                Assert.That(_mockTransport.Object.Configuration.Listeners.Count, Is.EqualTo(2));
            }
        }

        [TestFixture]
        public class Logging : ServiceBusTransportManagerTests
        {
            private string _loggedMessage;
            private readonly string TestQueueName = "Test Queue Name";

            [SetUp]
            public void LoggingSetup()
            {
                _mockEventLogger.Setup(m => m.Log(It.IsAny<SeverityLevel>(), It.IsAny<EventId>(), It.IsAny<Type>(), It.IsAny<string>()))
                    .Returns(Task.CompletedTask)
                    .Callback((SeverityLevel s, EventId eventId, Type type, string m) => _loggedMessage = m);

                _mockTransport.SetupGet(x => x.Configuration).Returns(new TransportConfiguration
                {
                    TransportConnection = new TransportConnection
                    {
                        ConnectionString = "Test Connection String",
                        QueueName = TestQueueName
                    }
                });
            }

            [Test]
            public void Warning_Event_Logged_If_Circuit_Breaker_Opens()
            {
                // Arrange
                var unused = new ServiceBusTransportManager(_mockTransport.Object, _mockCircuitBreaker.Object, _mockEventLogger.Object);
                var testException = new ApplicationException("Test Exception Message");
                var testDuration = new TimeSpan(10);

                // Act
                _mockCircuitBreaker.Raise(m => m.CircuitBreakerOpened += null, this, new CircuitBreakerOpenedEventArgs(testException, testDuration));

                // Assert
                Assert.That(_loggedMessage, Contains.Substring($"Circuit Breaker opened on queue '{TestQueueName}' for duration of {testDuration} due to {testException.Message}"));
            }

            [Test]
            public void Info_Event_Logged_If_Circuit_Breaker_Resets()
            {
                // Arrange
                var unused = new ServiceBusTransportManager(_mockTransport.Object, _mockCircuitBreaker.Object, _mockEventLogger.Object);

                // Act
                _mockCircuitBreaker.Raise(m => m.CircuitBreakerReset += null, new EventArgs());

                // Assert
                Assert.That(_loggedMessage, Contains.Substring("Circuit Breaker on queue 'Test Queue Name' reset"));
            }
        }
    }
}
