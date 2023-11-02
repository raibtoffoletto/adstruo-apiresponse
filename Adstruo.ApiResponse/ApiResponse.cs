using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Adstruo.ApiResponse;

public class ApiResponse : IActionResult
{
    protected readonly ILogger? _logger = null;

    public ApiResponse(ILogger? logger = null)
    {
        _logger = logger;
    }

    public virtual async Task ExecuteResultAsync(ActionContext context)
    {
        HttpResponse res = context.HttpContext.Response;

        ApiResult result = new();

        await res.JsonSerializerAsync(result, result.Code);
    }
}

public class ApiResponse<T> : ApiResponse
{
    private readonly Func<Task<T>> _action;

    public ApiResponse(Func<Task<T>> action, ILogger? logger = null)
        : base(logger)
    {
        _action = action;
    }

    public ApiResponse(Func<T> action, ILogger? logger = null)
        : base(logger)
    {
        _action = () => Task.Run(() => action());
    }

    public override async Task ExecuteResultAsync(ActionContext context)
    {
        HttpResponse res = context.HttpContext.Response;

        ApiResult<T> result = new();

        try
        {
            T data = await _action();

            result.SetData(data);
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

        await res.JsonSerializerAsync(result, result.Code);
    }
}
