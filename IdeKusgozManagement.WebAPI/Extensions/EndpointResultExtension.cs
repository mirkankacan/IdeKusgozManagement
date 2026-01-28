using IdeKusgozManagement.Application.Common;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace IdeKusgozManagement.WebAPI.Extensions
{
    public static class EndpointResultExtension
    {
        public static IActionResult ToActionResult<T>(this ServiceResult<T> serviceResult)
        {
            return serviceResult.StatusCode switch
            {
                HttpStatusCode.OK => new OkObjectResult(serviceResult.Data),
                HttpStatusCode.Created => new ObjectResult(serviceResult.Data)
                {
                    StatusCode = StatusCodes.Status201Created,
                    ContentTypes = { "application/json" }
                },
                HttpStatusCode.NotFound => new NotFoundObjectResult(serviceResult.Fail!),
                HttpStatusCode.BadRequest => new BadRequestObjectResult(serviceResult.Fail!),
                HttpStatusCode.Unauthorized => new UnauthorizedResult(),
                HttpStatusCode.Conflict => new ConflictObjectResult(serviceResult.Fail!),
                _ => new ObjectResult(serviceResult.Fail!)
                {
                    StatusCode = (int)serviceResult.StatusCode,
                    ContentTypes = { "application/problem+json" }
                }
            };
        }

        public static IActionResult ToActionResult(this ServiceResult serviceResult)
        {
            return serviceResult.StatusCode switch
            {
                HttpStatusCode.NoContent => new NoContentResult(),
                HttpStatusCode.NotFound => new NotFoundObjectResult(serviceResult.Fail!),
                HttpStatusCode.BadRequest => new BadRequestObjectResult(serviceResult.Fail!),
                HttpStatusCode.Unauthorized => new UnauthorizedResult(),
                _ => new ObjectResult(serviceResult.Fail!)
                {
                    StatusCode = (int)serviceResult.StatusCode,
                    ContentTypes = { "application/problem+json" }
                }
            };
        }
    }
}