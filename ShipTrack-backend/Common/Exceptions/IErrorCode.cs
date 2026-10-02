namespace ShipTrack_backend.Common.Exceptions
{
    public interface IErrorCode
    {
        int StatusCode { get; }
        string Message { get; }
        bool Success { get; }
    }
}
