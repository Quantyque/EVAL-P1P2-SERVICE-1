using Polly;
using Polly.Extensions.Http;
using Serilog;

namespace API.Extensions;

internal static class ResilienceExtensions
{
    /// <summary>
    /// Creates a retry policy for transient HTTP errors.
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy(IConfiguration configuration)
    {
        var retryCount = configuration.GetValue("Resilience:RetryCount", 3);
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(response => response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount,
                attempt => TimeSpan.FromMilliseconds(Math.Pow(2, attempt) * 100),
                (outcome, delay, attempt, _) =>
                {
                    Log.Warning("Retrying HTTP call. Attempt {Attempt} delayed {Delay}ms. StatusCode={StatusCode}",
                        attempt,
                        delay.TotalMilliseconds,
                        outcome.Result?.StatusCode);
                });
    }

    /// <summary>
    /// Creates a circuit breaker policy for repeated HTTP failures.
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> CreateCircuitBreakerPolicy(IConfiguration configuration)
    {
        var failures = configuration.GetValue("Resilience:CircuitBreakerFailures", 5);
        var duration = configuration.GetValue("Resilience:CircuitBreakerDurationSeconds", 30);

        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(failures, TimeSpan.FromSeconds(duration),
                (result, timespan) =>
                {
                    Log.Warning("Circuit broken for {Duration}s due to {Reason}", timespan.TotalSeconds, result.Exception?.Message ?? result.Result?.StatusCode.ToString());
                },
                () => Log.Information("Circuit closed. Resuming calls."));
    }

    /// <summary>
    /// Creates a timeout policy for outbound HTTP calls.
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> CreateTimeoutPolicy(IConfiguration configuration)
    {
        var timeout = configuration.GetValue("Resilience:TimeoutSeconds", 10);
        return Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(timeout));
    }
}
