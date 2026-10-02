using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace ShipTrack_backend.Common.Response
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = "Success";
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }

        public static ApiResponse<T> Ok(T data, string message = "Success", int statusCode = 200)
            => new() { Success = true, Message = message, Data = data, StatusCode = statusCode };

        public static ApiResponse Fail(List<string> errors, string message = "Error", int statusCode = 400)
            => new() { Success = false, Message = message, Errors = errors, StatusCode = statusCode };
    }
    public class ApiResponse : ApiResponse<object>
    {
        public static readonly string MESSAGE_ERROR_INPUT = "Dữ liệu không hợp lệ";

        public static ApiResponse ErrorInput(ModelStateDictionary modelState)
        {
            var errors = modelState.Values.SelectMany(x => x.Errors)
                .Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? "Invalid input" : e.ErrorMessage).ToList();
            return new() { Success = false, Message = MESSAGE_ERROR_INPUT, Errors = errors, StatusCode = 400 };
        }
        public static ApiResponse Ok(string message = "Success", int statusCode = 200)
         => new() { Success = true, Message = message, StatusCode = statusCode };
    }
}
