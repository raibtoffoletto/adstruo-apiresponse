namespace Adstruo.ApiResponse;

/// <summary>Structured API Result without data</summary>
public class ApiResult
{
    /// <summary>Request status: Ok/Error</summary>
    public string Status { get; set; } = ApiResultStatus.Ok;

    /// <summary>HTTP status for the request</summary>
    public int Code { get; set; } = 200;

    /// <summary>Error key in case of failure</summary>
    public string? Error { get; set; } = null;

    /// <summary>Message describing the error in case of failure</summary>
    public string? Message { get; set; } = null;

    /// <summary>Sets the result's error payload</summary>
    public ApiResult SetError(Exception ex)
    {
        return SetError(null, ex?.Message);
    }

    /// <summary>Sets the result's error payload</summary>
    public ApiResult SetError(ApiException ex)
    {
        return SetError(ex.Error, ex.Message, ex.Code);
    }

    /// <summary>Sets the result's error payload</summary>
    public ApiResult SetError(string? error = null, string? message = null, int code = 400)
    {
        Code = code;
        Status = ApiResultStatus.Error;
        Error = error ?? ApiExceptionError.UNKNOWN;
        Message = message ?? UnknownException.Description;

        return this;
    }

    /// <summary>Sets the result's status code</summary>
    public ApiResult SetCode(int code)
    {
        Code = code;

        return this;
    }
}

/// <summary>Structured API Result with data</summary>
public class ApiResult<T> : ApiResult
{
    /// <summary>Data to be returned</summary>
    public T? Data { get; set; } = default;

    /// <summary>Sets the result's Data</summary>
    public ApiResult<T> SetData(T data)
    {
        Data = data;

        return this;
    }
}
