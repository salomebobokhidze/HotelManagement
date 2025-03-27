public class ApiResponse
{
    public string Message { get; set; }
    public object Data { get; set; }
    public int StatusCode { get; set; }
    public bool Success { get; set; }

    public ApiResponse(string message, object data, int statusCode, bool success)
    {
        Message = message;
        Data = data;
        StatusCode = statusCode;
        Success = success;
    }
}
