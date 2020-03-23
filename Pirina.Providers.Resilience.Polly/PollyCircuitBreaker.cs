using Glasswall.Kernel.Resilience;
using Polly;
using Polly.CircuitBreaker;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Glasswall.Providers.Resilience.Polly
{
    public class PollyCircuitBreaker : ICircuitBreaker
    {
        private CircuitBreakerPolicy _circuitBreakerPolicy;
        private int _exceptionsAllowedBeforeBreaking;
        private TimeSpan _durationOfBreak;

        public event EventHandler<CircuitBreakerOpenedEventArgs> CircuitBreakerOpened;
        public event EventHandler CircuitBreakerReset;

        // this configuration should be supplied externally
        public PollyCircuitBreaker()
        : this(exceptionsAllowedBeforeBreaking: 5, durationOfBreak: TimeSpan.FromMinutes(5))
        {
        }

        public PollyCircuitBreaker(PollyCircuitBreakerConfiguration configuration)
        :this(configuration.ExceptionsAllowedBeforeBreaking, configuration.DurationOfBreak)
        {
        }

        private PollyCircuitBreaker(int exceptionsAllowedBeforeBreaking, TimeSpan durationOfBreak)
        {
            SetupCircuitBreaker(exceptionsAllowedBeforeBreaking, durationOfBreak);
        }

        private void SetupCircuitBreaker(int exceptionsAllowedBeforeBreaking, TimeSpan durationOfBreak)
        {
            _exceptionsAllowedBeforeBreaking = exceptionsAllowedBeforeBreaking;
            _durationOfBreak = durationOfBreak;

            _circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: _exceptionsAllowedBeforeBreaking,
                    durationOfBreak: _durationOfBreak,
                    onBreak: OnBreak,
                    onReset: OnReset);
        }

        private void OnBreak(Exception exception, TimeSpan duration)
        {
            CircuitBreakerOpened?.Invoke(this, new CircuitBreakerOpenedEventArgs(exception, duration));
        }

        private void OnReset()
        {
            CircuitBreakerReset?.Invoke(this, new EventArgs());
        }

        public async Task ExecuteAsync(Func<CancellationToken, Task> executeAction, Func<Exception, Task> failureAction, CancellationToken cancellationToken)
        {
            if (executeAction == null)
                throw new ArgumentNullException(nameof(executeAction), "an execute action must be specified");
            if (failureAction == null)
                throw new ArgumentNullException(nameof(failureAction), "a failure action must be specified");

            try
            {
                await _circuitBreakerPolicy.ExecuteAsync(executeAction, cancellationToken);
            }
            catch (Exception e)
            {
                await failureAction(e);
            }
        }
    }
}
