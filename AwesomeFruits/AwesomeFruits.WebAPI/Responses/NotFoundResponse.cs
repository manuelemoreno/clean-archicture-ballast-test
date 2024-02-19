using System.Text.Json.Serialization;

namespace AwesomeFruits.WebAPI.Responses;

public class NotFoundResponse
{
    public NotFoundResponse(string statusMessage)
    {
        StatusMessage = statusMessage;
    }

    [JsonPropertyName("msg")]
    public string StatusMessage { get; set; }
}