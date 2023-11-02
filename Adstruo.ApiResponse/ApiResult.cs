namespace Adstruo.ApiResponse;

public class ApiResult
{
    public string Status { get; set; } = ApiResultStatus.Ok;
    public int Code { get; set; } = 200;
    public string? Error { get; set; } = null;
    public string? Message { get; set; } = null;

    public ApiResult SetError(Exception ex)
    {
        return SetError(null, ex?.Message);
    }

    public ApiResult SetError(ApiException ex)
    {
        return SetError(ex.Error, ex.Message, ex.Code);
    }

    public ApiResult SetError(string? error = null, string? message = null, int code = 400)
    {
        Code = code;
        Status = ApiResultStatus.Error;
        Error = error ?? ApiExceptionError.UNKNOWN;
        Message = message ?? UnknownException.Description;

        return this;
    }
}

public class ApiResult<T> : ApiResult
{
    public T? Data { get; set; } = default;

    public ApiResult<T> SetData(T data)
    {
        Data = data;

        return this;
    }
}
