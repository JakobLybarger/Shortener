using Microsoft.AspNetCore.Mvc;
using Shortener.Models;
using Shortener.Utils;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var url = "http://localhost:5249";
var mappings = new Dictionary<string, string>();

app.MapPost("/shorten", ([FromBody] ShortenRequest request) =>
{

    var shortenedUrl = string.Empty;
    if (request.CustomShortenedUrl is null)
    {
        shortenedUrl = Utils.GenerateUrl();
    }
    else
    {
        if (mappings.ContainsKey(request.CustomShortenedUrl))
        {
            return Results.Conflict("This shortened URL already exists!");
        }

        shortenedUrl = request.CustomShortenedUrl;
    }

    mappings[shortenedUrl] = request.Url;
    return Results.Ok(new UrlResult { Url = $"{url}/{shortenedUrl}" });
});

app.MapGet("/{shortenedUrl}", (string shortenedUrl) =>
{
    if (!mappings.ContainsKey(shortenedUrl))
        return Results.NotFound("There is no mapping for this URL");

    return Results.Redirect(mappings[shortenedUrl]);
});

app.Run();
