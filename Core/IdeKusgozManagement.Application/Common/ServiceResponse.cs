namespace IdeKusgozManagement.Application.Common
{
    public class ServiceResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public static ServiceResponse<T> Success(T data, string message = "İşlem başarılı")
        {
            return new ServiceResponse<T>
            {
                IsSuccess = true,
                Message = message,
                Data = data
            };
        }

        public static ServiceResponse<T> Error(string message, List<string> errors = null)
        {
            return new ServiceResponse<T>
            {
                IsSuccess = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }

        public static ServiceResponse<T> Error(List<string> errors)
        {
            return new ServiceResponse<T>
            {
                IsSuccess = false,
                Message = "İşlem başarısız",
                Errors = errors
            };
        }
    }
}