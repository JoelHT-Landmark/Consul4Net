using System;

using Metrics;
using Metrics.Core;

namespace MyService
{
    public class RandomHealth : HealthCheck
    {
        public static Random Randomiser = new Random(DateTime.UtcNow.Millisecond);

        public RandomHealth() : base("Random")
        {}

        protected override HealthCheckResult Check()
        {
            var isHealthy = Randomiser.Next(100) > 75;

            return isHealthy ? 
                HealthCheckResult.Healthy() : 
                HealthCheckResult.Unhealthy("He's dead Jim!");
        }
    }
}
