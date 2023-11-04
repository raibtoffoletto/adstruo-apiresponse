using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Adstruo.ApiResponse;

/// <summary>Structured HttpResponse without data</summary>
public class ApiResponse : IActionResult
{
    protected readonly ILogger? _logger = null;
    protected readonly bool _isPublic = false;
    protected readonly object? _session = null;
    private readonly Func<Task>? _action = null;

    public ApiResponse(ILogger? logger = null, bool isPublic = true)
    {
        _logger = logger;
        _isPublic = isPublic;
    }

    public ApiResponse(object? session, ILogger? logger = null)
    {
        _logger = logger;
        _session = session;
    }

    public ApiResponse(Func<Task> action, ILogger? logger = null, bool isPublic = true)
    {
        _logger = logger;
        _isPublic = isPublic;
        _action = action;
    }

    public ApiResponse(Func<Task> action, object? session, ILogger? logger = null)
    {
        _logger = logger;
        _session = session;
        _action = action;
    }

    public ApiResponse(Action action, ILogger? logger = null, bool isPublic = true)
    {
        _logger = logger;
        _isPublic = isPublic;
        _action = () => Task.Run(() => action());
    }

    public ApiResponse(Action action, object? session, ILogger? logger = null)
    {
        _logger = logger;
        _session = session;
        _action = () => Task.Run(() => action());
    }

    public virtual async Task ExecuteResultAsync(ActionContext context)
    {
        HttpResponse res = context.HttpContext.Response;

        ApiResult result = new();

        try
        {
            if (!_isPublic & _session == null)
            {
                throw new UnauthenticatedException();
            }

            if (_action != null)
            {
                await _action();
            }
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

/// <summary>Structured HttpResponse with data</summary>
public class ApiResponse<T> : ApiResponse
{
    private readonly Func<Task<T>> _action;

    public ApiResponse(Func<Task<T>> action, ILogger? logger = null, bool isPublic = true)
        : base(logger, isPublic)
    {
        _action = action;
    }

    public ApiResponse(Func<Task<T>> action, object? session, ILogger? logger = null)
        : base(session, logger)
    {
        _action = action;
    }

    public ApiResponse(Func<T> action, ILogger? logger = null, bool isPublic = true)
        : base(logger, isPublic)
    {
        _action = () => Task.Run(() => action());
    }

    public ApiResponse(Func<T> action, object? session, ILogger? logger = null)
        : base(session, logger)
    {
        _action = () => Task.Run(() => action());
    }

    public override async Task ExecuteResultAsync(ActionContext context)
    {
        HttpResponse res = context.HttpContext.Response;

        ApiResult<T> result = new();

        try
        {
            if (!_isPublic & _session == null)
            {
                throw new UnauthenticatedException();
            }

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
