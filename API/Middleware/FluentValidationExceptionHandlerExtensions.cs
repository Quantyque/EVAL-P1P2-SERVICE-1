using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace API.Middleware;

/// <summary>
/// Provides an extension method to handle ValidationException globally and return standardized ProblemDetails responses.
/// </summary>
public static class FluentValidationExceptionHandlerExtensions
{
    /// <summary>
    /// Adds a global exception handler that intercepts ValidationException and returns a 400 ValidationProblem response.
    /// Falls back to a generic 500 ProblemDetails response for other unhandled exceptions.
    /// </summary>
    public static IApplicationBuilder UseFluentValidationExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var feature = context.Features.Get<IExceptionHandlerFeature>();
                var ex = feature?.Error;

                if (ex is ValidationException vex)
                {
                    var errors = vex.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await Results.ValidationProblem(errors).ExecuteAsync(context);
                    return;
                }

                await Results.Problem(statusCode: StatusCodes.Status500InternalServerError)
                    .ExecuteAsync(context);
            });
        });
        return app;
    }
}
