using System;

namespace Glasswall.Providers.Resilience.Polly
{
    public class PollyCircuitBreakerConfiguration
    {
        public int ExceptionsAllowedBeforeBreaking { get; set; } = 5;
        public TimeSpan DurationOfBreak { get; set; } = TimeSpan.FromMinutes(5);
    }
}
