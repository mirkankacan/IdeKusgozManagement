namespace IdeKusgozManagement.Application.Common
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public static ApiResponse<T> Success(T data, string message = "İşlem başarılı")
        {
            return new ApiResponse<T>
            {
                IsSuccess = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> Error(string message, List<string> errors = null)
        {
            return new ApiResponse<T>
            {
                IsSuccess = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }

        public static ApiResponse<T> Error(List<string> errors)
        {
            return new ApiResponse<T>
            {
                IsSuccess = false,
                Message = "İşlem başarısız",
                Errors = errors
            };
        }
    }
}