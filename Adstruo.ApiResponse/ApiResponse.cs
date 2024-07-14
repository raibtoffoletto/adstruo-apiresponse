using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Adstruo.ApiResponse;

/// <summary>Structured HttpResponse without data</summary>
public class ApiResponse : IActionResult
{
    /// <summary>ILogger instance</summary>
    protected readonly ILogger? _logger = null;

    /// <summary>Define if route is public or not</summary>
    protected readonly bool _isPublic = false;

    /// <summary>An authorized session</summary>
    protected readonly object? _session = null;

    /// <summary>The success status code</summary>
    protected readonly int? _code = null;

    private readonly Func<Task>? _action = null;

    /// <summary>Public ApiResponse without action</summary>
    public ApiResponse(ILogger? logger = null, bool isPublic = true, int? code = null)
    {
        _logger = logger;
        _isPublic = isPublic;
        _code = code;
    }

    /// <summary>Private ApiResponse without action</summary>
    public ApiResponse(object? session, ILogger? logger = null, int? code = null)
    {
        _logger = logger;
        _session = session;
        _code = code;
    }

    /// <summary>Public ApiResponse with async action</summary>
    public ApiResponse(
        Func<Task> action,
        ILogger? logger = null,
        bool isPublic = true,
        int? code = null
    )
    {
        _logger = logger;
        _isPublic = isPublic;
        _action = action;
        _code = code;
    }

    /// <summary>Private ApiResponse with async action</summary>
    public ApiResponse(Func<Task> action, object? session, ILogger? logger = null, int? code = null)
    {
        _logger = logger;
        _session = session;
        _action = action;
        _code = code;
    }

    /// <summary>Public ApiResponse with action</summary>
    public ApiResponse(
        Action action,
        ILogger? logger = null,
        bool isPublic = true,
        int? code = null
    )
    {
        _logger = logger;
        _isPublic = isPublic;
        _action = () => Task.Run(() => action());
        _code = code;
    }

    /// <summary>Private ApiResponse with action</summary>
    public ApiResponse(Action action, object? session, ILogger? logger = null, int? code = null)
    {
        _logger = logger;
        _session = session;
        _action = () => Task.Run(() => action());
        _code = code;
    }

    /// <summary>Sets the HttpResponse</summary>
    public virtual async Task ExecuteResultAsync(ActionContext context)
    {
        HttpResponse res = context.HttpContext.Response;

        ApiResult result = new();

        if (!_isPublic & _session == null)
        {
            throw new UnauthenticatedException();
        }

        if (_action != null)
        {
            await _action();
        }

        if (_code != null)
        {
            result.SetCode((int)_code);
        }

        await res.JsonSerializerAsync(result, result.Code);
    }
}

/// <summary>Structured HttpResponse with data</summary>
public class ApiResponse<T> : ApiResponse
{
    private readonly Func<Task<T>> _action;

    /// <summary>Public ApiResponse with async action and data</summary>
    public ApiResponse(
        Func<Task<T>> action,
        ILogger? logger = null,
        bool isPublic = true,
        int? code = null
    )
        : base(logger, isPublic, code)
    {
        _action = action;
    }

    /// <summary>Private ApiResponse with async action and data</summary>
    public ApiResponse(
        Func<Task<T>> action,
        object? session,
        ILogger? logger = null,
        int? code = null
    )
        : base(session, logger, code)
    {
        _action = action;
    }

    /// <summary>Public ApiResponse with action and data</summary>
    public ApiResponse(
        Func<T> action,
        ILogger? logger = null,
        bool isPublic = true,
        int? code = null
    )
        : base(logger, isPublic, code)
    {
        _action = () => Task.Run(() => action());
    }

    /// <summary>Private ApiResponse with action and data</summary>
    public ApiResponse(Func<T> action, object? session, ILogger? logger = null, int? code = null)
        : base(session, logger, code)
    {
        _action = () => Task.Run(() => action());
    }

    /// <summary>Sets the HttpResponse</summary>
    public override async Task ExecuteResultAsync(ActionContext context)
    {
        HttpResponse res = context.HttpContext.Response;

        ApiResult<T> result = new();

        if (!_isPublic & _session == null)
        {
            throw new UnauthenticatedException();
        }

        if (_code != null)
        {
            result.SetCode((int)_code);
        }

        T data = await _action();

        result.SetData(data);

        await res.JsonSerializerAsync(result, result.Code);
    }
}
