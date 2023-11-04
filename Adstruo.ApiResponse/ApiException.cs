namespace Adstruo.ApiResponse;

/// <summary>Collection of error keys</summary>
public static class ApiExceptionError
{
    /// <summary>ALL_FIELDS_REQUIRED</summary>
    public const string ALL_FIELDS_REQUIRED = "ALL_FIELDS_REQUIRED";

    /// <summary>BAD_REQUEST</summary>
    public const string BAD_REQUEST = "BAD_REQUEST";

    /// <summary>FAILED_TO_PARSE_REQUEST</summary>
    public const string FAILED_TO_PARSE_REQUEST = "FAILED_TO_PARSE_REQUEST";

    /// <summary>FORBIDDEN</summary>
    public const string FORBIDDEN = "FORBIDDEN";

    /// <summary>INVALID_USER_CREDENTIALS</summary>
    public const string INVALID_USER_CREDENTIALS = "INVALID_USER_CREDENTIALS";

    /// <summary>NETWORK_ERROR</summary>
    public const string NETWORK_ERROR = "NETWORK_ERROR";

    /// <summary>NOT_FOUND</summary>
    public const string NOT_FOUND = "NOT_FOUND";

    /// <summary>UNAUTHENTICATED</summary>
    public const string UNAUTHENTICATED = "UNAUTHENTICATED";

    /// <summary>UNAUTHORIZED</summary>
    public const string UNAUTHORIZED = "UNAUTHORIZED";

    /// <summary>UNKNOWN</summary>
    public const string UNKNOWN = "UNKNOWN";
}

/// <summary>Extended exception with HTTP error code and defined error key</summary>
public abstract class ApiException : Exception
{
    /// <summary>HTTP error code</summary>
    public abstract int Code { get; }

    /// <summary>Error key</summary>
    public abstract string Error { get; }

    /// <summary>ApiException</summary>
    public ApiException(string? message)
        : base(message) { }
}

/// <summary>AllFieldsRequiredException</summary>
public class AllFieldsRequiredException : ApiException
{
    /// <summary>Error description</summary>
    public static readonly string Description =
        "The required fields for this method were not provided";

    /// <summary>Http error code 406</summary>
    public override int Code { get; } = 406;

    /// <summary>Error key ALL_FIELDS_REQUIRED</summary>
    public override string Error { get; } = ApiExceptionError.ALL_FIELDS_REQUIRED;

    /// <summary>Define custom message</summary>
    public AllFieldsRequiredException(string? message = null)
        : base(message ?? Description) { }
}

/// <summary>BadRequestException</summary>
public class BadRequestException : ApiException
{
    /// <summary>Error description</summary>
    public static readonly string Description = "Something is wrong, is it me? Or you?";

    /// <summary>Http error code 400</summary>
    public override int Code { get; } = 400;

    /// <summary>Error key BAD_REQUEST</summary>
    public override string Error { get; } = ApiExceptionError.BAD_REQUEST;

    /// <summary>Define custom message</summary>
    public BadRequestException(string? message = null)
        : base(message ?? Description) { }
}

/// <summary>FailedToParseRequestException</summary>
public class FailedToParseRequestException : ApiException
{
    /// <summary>Error description</summary>
    public static readonly string Description = "Failed to properly parse this request' contents";

    /// <summary>Http error code 422</summary>
    public override int Code { get; } = 422;

    /// <summary>Error key FAILED_TO_PARSE_REQUEST</summary>
    public override string Error { get; } = ApiExceptionError.FAILED_TO_PARSE_REQUEST;

    /// <summary>Define custom message</summary>
    public FailedToParseRequestException(string? message = null)
        : base(message ?? Description) { }
}

/// <summary>ForbiddenException</summary>
public class ForbiddenException : ApiException
{
    /// <summary>Error description</summary>
    public static readonly string Description = "Access forbidden to this resource";

    /// <summary>Http error code 403</summary>
    public override int Code { get; } = 403;

    /// <summary>Error key FORBIDDEN</summary>
    public override string Error { get; } = ApiExceptionError.FORBIDDEN;

    /// <summary>Define custom message</summary>
    public ForbiddenException(string? message = null)
        : base(message ?? Description) { }
}

/// <summary>InvalidUserCredentialsException</summary>
public class InvalidUserCredentialsException : ApiException
{
    /// <summary>Error description</summary>
    public static readonly string Description = "The provided user credentials are invalid";

    /// <summary>Http error code 400</summary>
    public override int Code { get; } = 400;

    /// <summary>Error key INVALID_USER_CREDENTIALS</summary>
    public override string Error { get; } = ApiExceptionError.INVALID_USER_CREDENTIALS;

    /// <summary>Define custom message</summary>
    public InvalidUserCredentialsException(string? message = null)
        : base(message ?? Description) { }
}

/// <summary>NetworkErrorException</summary>
public class NetworkErrorException : ApiException
{
    /// <summary>Error description</summary>
    public static readonly string Description = "Network service unreachable";

    /// <summary>Http error code 503</summary>
    public override int Code { get; } = 503;

    /// <summary>Error key NETWORK_ERROR</summary>
    public override string Error { get; } = ApiExceptionError.NETWORK_ERROR;

    /// <summary>Define custom message</summary>
    public NetworkErrorException(string? message = null)
        : base(message ?? Description) { }
}

/// <summary>NotFoundException</summary>
public class NotFoundException : ApiException
{
    /// <summary>Error description</summary>
    public static readonly string Description = "Couldn't find what you were looking for";

    /// <summary>Http error code 404</summary>
    public override int Code { get; } = 404;

    /// <summary>Error key NOT_FOUND</summary>
    public override string Error { get; } = ApiExceptionError.NOT_FOUND;

    /// <summary>Define custom message</summary>
    public NotFoundException(string? message = null)
        : base(message ?? Description) { }
}

/// <summary>UnauthenticatedException</summary>
public class UnauthenticatedException : ApiException
{
    /// <summary>Error description</summary>
    public static readonly string Description = "User unauthenticated for this request";

    /// <summary>Http error code 401</summary>
    public override int Code { get; } = 401;

    /// <summary>Error key UNAUTHENTICATED</summary>
    public override string Error { get; } = ApiExceptionError.UNAUTHENTICATED;

    /// <summary>Define custom message</summary>
    public UnauthenticatedException(string? message = null)
        : base(message ?? Description) { }
}

/// <summary>UnauthorizedException</summary>
public class UnauthorizedException : ApiException
{
    /// <summary>Error description</summary>
    public static readonly string Description = "User unauthorized for this request";

    /// <summary>Http error code 401</summary>
    public override int Code { get; } = 401;

    /// <summary>Error key UNAUTHORIZED</summary>
    public override string Error { get; } = ApiExceptionError.UNAUTHORIZED;

    /// <summary>Define custom message</summary>
    public UnauthorizedException(string? message = null)
        : base(message ?? Description) { }
}

/// <summary>UnknownException</summary>
public class UnknownException : ApiException
{
    /// <summary>Error description</summary>
    public static readonly string Description = "An unknown error occurred";

    /// <summary>Http error code 500</summary>
    public override int Code { get; } = 500;

    /// <summary>Error key UNKNOWN</summary>
    public override string Error { get; } = ApiExceptionError.UNKNOWN;

    /// <summary>Define custom message</summary>
    public UnknownException(string? message = null)
        : base(message ?? Description) { }
}
