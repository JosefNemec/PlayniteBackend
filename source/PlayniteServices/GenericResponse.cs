namespace PlayniteServices;

public class ResponseBase
{
    public string? Error { get; set; }
}

public class ErrorResponse : ResponseBase
{
    public ErrorResponse(string error)
    {
        Error = error;
    }

    public ErrorResponse(Exception error)
    {
        Error = error.Message;
    }
}

public class DataResponse<T> : ResponseBase
{
    public T? Data { get; set; }

    public DataResponse()
    {
    }

    public DataResponse(T? data)
    {
        Data = data;
    }
}
