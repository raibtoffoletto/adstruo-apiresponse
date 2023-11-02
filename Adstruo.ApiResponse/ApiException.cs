namespace Adstruo.ApiResponse;

public static class ApiExceptionError
{
    public static readonly string ALL_FIELDS_REQUIRED = "ALL_FIELDS_REQUIRED";
    public static readonly string BAD_REQUEST = "BAD_REQUEST";
    public static readonly string FAILED_TO_PARSE_REQUEST = "FAILED_TO_PARSE_REQUEST";
    public static readonly string FORBIDDEN = "FORBIDDEN";
    public static readonly string INVALID_USER_CREDENTIALS = "INVALID_USER_CREDENTIALS";
    public static readonly string NETWORK_ERROR = "NETWORK_ERROR";
    public static readonly string NOT_FOUND = "NOT_FOUND";
    public static readonly string UNAUTHENTICATED = "UNAUTHENTICATED";
    public static readonly string UNAUTHORIZED = "UNAUTHORIZED";
    public static readonly string UNKNOWN = "UNKNOWN";
}

public abstract class ApiException : Exception
{
    public abstract int Code { get; }
    public abstract string Error { get; }

    public ApiException(string? message)
        : base(message) { }
}

public class AllFieldsRequiredException : ApiException
{
    public static readonly string Description =
        "The required fields for this method were not provided";
    public override int Code { get; } = 406;
    public override string Error { get; } = ApiExceptionError.ALL_FIELDS_REQUIRED;

    public AllFieldsRequiredException(string? message = null)
        : base(message ?? Description) { }
}

public class BadRequestException : ApiException
{
    public static readonly string Description = "Something is wrong, is it me? Or you?";
    public override int Code { get; } = 400;
    public override string Error { get; } = ApiExceptionError.BAD_REQUEST;

    public BadRequestException(string? message = null)
        : base(message ?? Description) { }
}

public class FailedToParseRequestException : ApiException
{
    public static readonly string Description = "Failed to properly parse this request' contents";
    public override int Code { get; } = 422;
    public override string Error { get; } = ApiExceptionError.FAILED_TO_PARSE_REQUEST;

    public FailedToParseRequestException(string? message = null)
        : base(message ?? Description) { }
}

public class ForbiddenException : ApiException
{
    public static readonly string Description = "Access forbidden to this resource";
    public override int Code { get; } = 403;
    public override string Error { get; } = ApiExceptionError.FORBIDDEN;

    public ForbiddenException(string? message = null)
        : base(message ?? Description) { }
}

public class InvalidUserCredentialsException : ApiException
{
    public static readonly string Description = "The provided user credentials are invalid";
    public override int Code { get; } = 400;
    public override string Error { get; } = ApiExceptionError.INVALID_USER_CREDENTIALS;

    public InvalidUserCredentialsException(string? message = null)
        : base(message ?? Description) { }
}

public class NetworkErrorException : ApiException
{
    public static readonly string Description = "Network service unreachable";
    public override int Code { get; } = 503;
    public override string Error { get; } = ApiExceptionError.NETWORK_ERROR;

    public NetworkErrorException(string? message = null)
        : base(message ?? Description) { }
}

public class NotFoundException : ApiException
{
    public static readonly string Description = "Couldn't find what you were looking for";
    public override int Code { get; } = 404;
    public override string Error { get; } = ApiExceptionError.NOT_FOUND;

    public NotFoundException(string? message = null)
        : base(message ?? Description) { }
}

public class UnauthenticatedException : ApiException
{
    public static readonly string Description = "User unauthenticated for this request";
    public override int Code { get; } = 401;
    public override string Error { get; } = ApiExceptionError.UNAUTHENTICATED;

    public UnauthenticatedException(string? message = null)
        : base(message ?? Description) { }
}

public class UnauthorizedException : ApiException
{
    public static readonly string Description = "User unauthorized for this request";
    public override int Code { get; } = 401;
    public override string Error { get; } = ApiExceptionError.UNAUTHORIZED;

    public UnauthorizedException(string? message = null)
        : base(message ?? Description) { }
}

public class UnknownException : ApiException
{
    public static readonly string Description = "An unknown error occurred";
    public override int Code { get; } = 500;
    public override string Error { get; } = ApiExceptionError.UNKNOWN;

    public UnknownException(string? message = null)
        : base(message ?? Description) { }
}
