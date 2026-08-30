using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Behaviors;

public class LoggingBehavior<TRequest , TResponse> : IPipelineBehavior<TRequest , TResponse>
where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // log the request name 
        var requestName = request.GetType().Name;
        _logger.LogInformation("Handling {RequestName}", requestName);
        
        // Measure and log Execution Time.
        var stopWatch = Stopwatch.StartNew();
        var response = await next(cancellationToken);
        stopWatch.Stop();
        
        _logger.LogInformation("{RequestName} executed in {ElapsedMilliseconds} ms",
                                requestName, stopWatch.ElapsedMilliseconds);

        return response;
    }
}