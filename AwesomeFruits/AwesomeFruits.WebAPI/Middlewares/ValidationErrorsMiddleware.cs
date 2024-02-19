using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AwesomeFruits.WebAPI.Exceptions;
using AwesomeFruits.WebAPI.Responses;
using Microsoft.AspNetCore.Http;

namespace AwesomeFruits.WebAPI.Middlewares;

public class ValidationErrorsMiddleware
{
    private readonly RequestDelegate _next;

    public ValidationErrorsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationErrorsException ex)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var validationErrorsResponse =
                new ValidationErrorsResponse(ex.Errors.Select(error => error).ToList());

            var validationErrorsSerialized = JsonSerializer.Serialize(validationErrorsResponse);

            response.StatusCode = 400;
            await response.WriteAsync(validationErrorsSerialized);
        }
    }
}