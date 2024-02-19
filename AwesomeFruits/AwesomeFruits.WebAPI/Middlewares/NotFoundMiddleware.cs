using System.Text.Json;
using System.Threading.Tasks;
using AwesomeFruits.Domain.Exceptions;
using AwesomeFruits.WebAPI.Responses;
using Microsoft.AspNetCore.Http;

namespace AwesomeFruits.WebAPI.Middlewares;

public class NotFoundMiddleware
{
    private readonly RequestDelegate _next;

    public NotFoundMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (EntityNotFoundException ex)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var notFoundResponse = new NotFoundResponse(ex.Message);

            var serializeNotFoundResponse = JsonSerializer.Serialize(notFoundResponse);

            response.StatusCode = 404;
            await response.WriteAsync(serializeNotFoundResponse);
        }
    }
}