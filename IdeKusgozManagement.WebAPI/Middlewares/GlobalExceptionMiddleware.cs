using IdeKusgozManagement.Infrastructure.Helpers;
using Microsoft.AspNetCore.Mvc;
using Serilog.Context;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace IdeKusgozManagement.WebAPI.Middlewares
{
    public class GlobalExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            SetLogContext(context);

            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await LogAndHandleExceptionAsync(context, ex);
            }
        }

        private void SetLogContext(HttpContext context)
        {
            var request = context.Request;

            var userId = context.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? null;
            var clientIp = IpHelper.GetClientIp(_httpContextAccessor);
            var userAgent = IpHelper.GetUserAgent(_httpContextAccessor);
            var requestId = context.TraceIdentifier;
            var action = $"{request.Method} {request.Path}";
            var endpoint = context.GetEndpoint();
            var controllerActionDescriptor = endpoint?.Metadata
                .GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>();

            var module = controllerActionDescriptor?.ControllerName + "Controller" ?? "Unknown";


            LogContext.PushProperty("UserId", userId);
            LogContext.PushProperty("Action", action);
            LogContext.PushProperty("Module", module);
            LogContext.PushProperty("ClientIP", clientIp);
            LogContext.PushProperty("UserAgent", userAgent);
            LogContext.PushProperty("RequestId", requestId);
        }


        private async Task LogAndHandleExceptionAsync(HttpContext context, Exception exception)
        {
            var request = context.Request;

            // LogContext zaten SetLogContext() tarafından set edildi
            _logger.LogError(exception,
                "HTTP {Method} {Path} başarısız oldu. QueryString: {QueryString}",
                request.Method,
                request.Path,
                request.QueryString.ToString());

            await HandleExceptionAsync(context, exception);
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var problemDetails = exception switch
            {
                UnauthorizedAccessException => CreateUnauthorizedProblemDetails(context),
                KeyNotFoundException => CreateNotFoundProblemDetails(context, exception),
                ArgumentNullException => CreateBadRequestProblemDetails(context, exception),
                ArgumentException => CreateBadRequestProblemDetails(context, exception),
                InvalidOperationException => CreateBadRequestProblemDetails(context, exception),
                NotImplementedException => CreateNotImplementedProblemDetails(context),
                TimeoutException => CreateTimeoutProblemDetails(context),
                OperationCanceledException => CreateRequestCancelledProblemDetails(context),
                _ => CreateInternalServerErrorProblemDetails(context)
            };

            context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(problemDetails, options);
            await context.Response.WriteAsync(json);
        }


        private static ProblemDetails CreateUnauthorizedProblemDetails(HttpContext context)
        {
            return new ProblemDetails
            {
                Title = "Yetkisiz Erişim",
                Detail = "Bu kaynağa erişim yetkiniz yok",
                Status = (int)HttpStatusCode.Unauthorized,
                Instance = context.Request.Path
            };
        }

        private static ProblemDetails CreateNotFoundProblemDetails(HttpContext context, Exception exception)
        {
            return new ProblemDetails
            {
                Title = "Kayıt Bulunamadı",
                Detail = exception.Message,
                Status = (int)HttpStatusCode.NotFound,
                Instance = context.Request.Path
            };
        }

        private static ProblemDetails CreateBadRequestProblemDetails(HttpContext context, Exception exception)
        {
            return new ProblemDetails
            {
                Title = "Geçersiz İstek",
                Detail = exception.Message,
                Status = (int)HttpStatusCode.BadRequest,
                Instance = context.Request.Path
            };
        }

        private static ProblemDetails CreateNotImplementedProblemDetails(HttpContext context)
        {
            return new ProblemDetails
            {
                Title = "Özellik Uygulanmadı",
                Detail = "Bu özellik henüz uygulanmamıştır",
                Status = (int)HttpStatusCode.NotImplemented,
                Instance = context.Request.Path
            };
        }

        private static ProblemDetails CreateTimeoutProblemDetails(HttpContext context)
        {
            return new ProblemDetails
            {
                Title = "İstek Zaman Aşımına Uğradı",
                Detail = "İşlem çok uzun sürdü ve zaman aşımına uğradı",
                Status = (int)HttpStatusCode.RequestTimeout,
                Instance = context.Request.Path
            };
        }

        private static ProblemDetails CreateRequestCancelledProblemDetails(HttpContext context)
        {
            return new ProblemDetails
            {
                Title = "İstek İptal Edildi",
                Detail = "İstek kullanıcı tarafından iptal edildi",
                Status = 499,
                Instance = context.Request.Path
            };
        }

        private static ProblemDetails CreateInternalServerErrorProblemDetails(HttpContext context)
        {
            return new ProblemDetails
            {
                Title = "Sunucu Hatası",
                Detail = "Beklenmeyen bir hata oluştu",
                Status = (int)HttpStatusCode.InternalServerError,
                Instance = context.Request.Path
            };
        }
    }


}
