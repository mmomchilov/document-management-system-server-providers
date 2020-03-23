using System;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.Kernel.Resilience;
using Moq;
using NUnit.Framework;

namespace Glasswall.Providers.Resilience.Polly.Tests.L0
{
    [TestFixture]
    [Category("L0")]
    public class CircuitBreakerTests
    {
        private CancellationTokenSource _cts;

        [SetUp]
        public void CircuitBreakerTestsSetUp()
        {
            _cts = new CancellationTokenSource();
        }

        [TestFixture]
        public class ConstructorTests : CircuitBreakerTests
        {
            [Test]
            public void Default_Constructor()
            {
                // Act
                var circuitBreaker = new PollyCircuitBreaker();

                // Assert
                Assert.That(circuitBreaker, Is.Not.Null);
            }

            [Test]
            public void Separate_Configuration_Item_Constructor()
            {
                // arrange 
                const int testNumberOfPermittedExceptions = 5;
                var testBreakerDuration = TimeSpan.FromSeconds(1);
                var configuration = new PollyCircuitBreakerConfiguration
                {
                    ExceptionsAllowedBeforeBreaking = testNumberOfPermittedExceptions,
                    DurationOfBreak = testBreakerDuration
                };

                // Act
                var circuitBreaker = new PollyCircuitBreaker(configuration);

                // Assert
                Assert.That(circuitBreaker, Is.Not.Null);
            }
        }

        [TestFixture]
        public class ExecuteAsyncTests : CircuitBreakerTests
        {
            private ICircuitBreaker _circuitBreaker;

            [SetUp]
            public void ExecuteAsyncTestsSetUp()
            {
                _circuitBreaker = new PollyCircuitBreaker();
            }

            [Test]
            public void Null_ExecuteAction_Throws_ArgumentNullException()
            {
                // Act
                var thrownException = Assert.ThrowsAsync<ArgumentNullException>(async () => 
                    await _circuitBreaker.ExecuteAsync(null, Mock.Of<Func<Exception, Task>>(), _cts.Token));

                // Assert
                Assert.That(thrownException.ParamName, Is.EqualTo("executeAction"));
                Assert.That(thrownException.Message, Contains.Substring("an execute action must be specified"));

            }

            [Test]
            public void Null_FailureAction_Throws_InvalidArgument_Exception()
            {
                // Act
                var thrownException = Assert.ThrowsAsync<ArgumentNullException>(async () => 
                    await _circuitBreaker.ExecuteAsync(Mock.Of<Func<CancellationToken, Task>>(), null, _cts.Token));

                // Assert
                Assert.That(thrownException.ParamName, Is.EqualTo("failureAction"));
                Assert.That(thrownException.Message, Contains.Substring("a failure action must be specified"));
            }

            [Test]
            public async Task Circuit_Breaker_Runs_Specified_Action()
            {
                // Arrange 
                var specifiedActionExecuted = false;
                Task TestAction(CancellationToken token)
                {
                    specifiedActionExecuted = true;
                    return Task.CompletedTask;
                }

                // Act
                await _circuitBreaker.ExecuteAsync(TestAction, Mock.Of<Func<Exception, Task>>(), _cts.Token);

                // Assert
                Assert.That(specifiedActionExecuted, Is.True);
            }

            [Test]
            public async Task Circuit_Breaker_Runs_FailureAction_When_Exception_Occurs_In_Specified_Action()
            {
                // Arrange 
                var allowedExceptionCount = 2;
                var breakerDuration = TimeSpan.FromSeconds(2);

                Task TestAction(CancellationToken token)
                {
                    throw new ApplicationException();
                }

                Exception receivedException = null;
                Task TestFailureAction(Exception ex)
                {
                    receivedException = ex;
                    return Task.CompletedTask;
                }
                var configuration = new PollyCircuitBreakerConfiguration
                {
                    ExceptionsAllowedBeforeBreaking = allowedExceptionCount,
                    DurationOfBreak = breakerDuration
                };

                var testCircuitBreaker = new PollyCircuitBreaker(configuration);

                // Act
                await testCircuitBreaker.ExecuteAsync(TestAction, TestFailureAction, _cts.Token);

                // Assert
                Assert.That(receivedException, Is.Not.Null);
                Assert.That(receivedException, Is.TypeOf<ApplicationException>());
            }

            [Test]
            public async Task Specified_Action_Is_Not_Executed_Once_Exception_Threshold_Is_Exceeded()
            {
                // Arrange 
                OpenCircuitBreaker(_circuitBreaker);

                var testActionExecuted = false;
                Task TestAction(CancellationToken token)
                {
                    testActionExecuted = true;
                    return Task.CompletedTask;
                }

                // Act
                await _circuitBreaker.ExecuteAsync(TestAction, (ex) => Task.CompletedTask, _cts.Token);

                // Assert   
                Assert.That(testActionExecuted, Is.False);
            }
 
            [Test]
            public async Task Specified_Action_Is_Executed_Once_Breaker_Duration_Has_Expired_On_Open_Breaker()
            {
                // Arrange 
                const int exceptionsAllowedBeforeBreaking = 5;
                var breakDuration = TimeSpan.FromSeconds(1);
                var configuration = new PollyCircuitBreakerConfiguration
                {
                    ExceptionsAllowedBeforeBreaking = exceptionsAllowedBeforeBreaking,
                    DurationOfBreak = breakDuration
                };
                var testCircuitBreaker = new PollyCircuitBreaker(configuration);
                OpenCircuitBreaker(_circuitBreaker);

                var testActionExecuted = false;
                Task TestAction(CancellationToken token)
                {
                    testActionExecuted = true;
                    return Task.CompletedTask;
                }
                // Act
                Thread.Sleep(breakDuration);
                await testCircuitBreaker.ExecuteAsync(TestAction, exception => Task.CompletedTask, _cts.Token);

                // Assert  
                Assert.That(testActionExecuted, Is.True);
            }

            private void OpenCircuitBreaker(ICircuitBreaker circuitBreaker)
            {
                var circuitOpened = false;
                circuitBreaker.CircuitBreakerOpened += (sender, args) => circuitOpened = true;

                while (!circuitOpened)
                {
                    circuitBreaker.ExecuteAsync(token => throw new ApplicationException(),
                        exception => Task.CompletedTask,
                        _cts.Token);
                }
            }
        }

        [TestFixture]
        public class EventHandling : CircuitBreakerTests
        {
            [Test]
            public async Task OnBreak_Event_Is_Raised_When_Circuit_Breaker_Opens()
            {
                // Arrange 
                const int exceptionsAllowedThreshold = 1;
                var breakDuration = TimeSpan.FromSeconds(1);
                var circuitOpenEventTriggered = false;
                var configuration = new PollyCircuitBreakerConfiguration
                {
                    ExceptionsAllowedBeforeBreaking = exceptionsAllowedThreshold,
                    DurationOfBreak = breakDuration
                };
                var circuitBreaker = new PollyCircuitBreaker(configuration);
                circuitBreaker.CircuitBreakerOpened += (sender, args) => circuitOpenEventTriggered = true;

                // Act
                await ThrowExceptionsInCircuitBreaker(circuitBreaker, numberOfExceptions:exceptionsAllowedThreshold);

                // Assert    
                Assert.That(circuitOpenEventTriggered, Is.True, "the circuit open event should be triggered");
            }

            [Test]
            public async Task OnReset_Event_Is_Raised_When_ExecuteAction_Completes_Successfully_After_Break_Duration_Has_Expired()
            {
                // Arrange 
                const int exceptionsAllowedThreshold = 1;
                TimeSpan breakDuration = TimeSpan.FromSeconds(1);
                var configuration = new PollyCircuitBreakerConfiguration
                {
                    ExceptionsAllowedBeforeBreaking = exceptionsAllowedThreshold,
                    DurationOfBreak = breakDuration
                };
                var circuitResetEventTriggered = false;
                var circuitBreaker = new PollyCircuitBreaker(configuration);
                circuitBreaker.CircuitBreakerReset += (sender, args) => circuitResetEventTriggered = true;

                // Act
                await ThrowExceptionsInCircuitBreaker(circuitBreaker, numberOfExceptions: exceptionsAllowedThreshold);
                Thread.Sleep(breakDuration);
                await circuitBreaker.ExecuteAsync(
                    token => Task.CompletedTask,
                    exception => Task.CompletedTask,
                    _cts.Token);

                // Assert   
                Assert.That(circuitResetEventTriggered, Is.True, "the Circuit Breaker Reset Event should be triggered");
            }

            private async Task ThrowExceptionsInCircuitBreaker(ICircuitBreaker circuitBreaker, int numberOfExceptions)
            {
                for (var i = 0; i < numberOfExceptions; i++)
                {
                    await circuitBreaker.ExecuteAsync(
                        token => throw new ApplicationException(),
                        exception => Task.CompletedTask,
                        _cts.Token);
                }
            }
        }


    }
}
