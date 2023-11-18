using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Adstruo.ApiResponse;

/// <summary>Catch all exceptions middleware</summary>
public class ApiExceptionMiddleware
{
    private readonly ILogger<ApiExceptionMiddleware> _logger;
    private readonly RequestDelegate _next;

    /// <summary>Creates the catch all exceptions middleware</summary>
    public ApiExceptionMiddleware(ILogger<ApiExceptionMiddleware> logger, RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    /// <summary>Execute the catch all exceptions middleware</summary>
    public async Task InvokeAsync(HttpContext context)
    {
        ApiResult result = new();

        try
        {
            await _next(context);

            return;
        }
        catch (ApiException ex)
        {
            result.SetError(ex);

            _logger?.LogDebug("[Result Error]: {message}", ex.Message);
        }
        catch (Exception ex)
        {
            result.SetError(ex);

            _logger?.LogDebug("[Result Error]: {message}", ex.Message);
        }

        await context.Response.JsonSerializerAsync(result, result.Code);
    }
}
