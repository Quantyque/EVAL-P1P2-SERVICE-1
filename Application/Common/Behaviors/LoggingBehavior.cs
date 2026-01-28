using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Common.Behaviors;

/// <summary>
/// Logs MediatR requests and their responses, including execution time and long-running warnings.
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Creates the logging behavior.
    /// </summary>
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Logs the request, measures execution time, and logs the response.
    /// </summary>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation("Handling {RequestName} with payload {@Request}", requestName, request);

        try
        {
            var response = await next(cancellationToken).WaitAsync(cancellationToken);

            stopwatch.Stop();
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            if (elapsedMilliseconds > 500)
            {
                _logger.LogWarning("Long Running Request: {RequestName} ({ElapsedMilliseconds} milliseconds) {@Request}",
                    requestName, elapsedMilliseconds, request);
            }

            _logger.LogInformation("Handled {RequestName} in {ElapsedMilliseconds}ms with response {@Response}",
                requestName, elapsedMilliseconds, response);

            return response;
        }
        catch (Exception)
        {
            stopwatch.Stop();
            throw;
        }
    }
}
