using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shorten.Data;
using Shorten.Data.Data;
using Shortener.Models;
using Shortener.Utils;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Shorten");
builder.Services.AddDbContext<ShortenContext>(options => options.UseSqlServer(connectionString));

var app = builder.Build();

var url = "http://localhost:5249";
var mappings = new Dictionary<string, string>();

app.MapPost("/shorten", async (
    [FromBody] ShortenRequest request,
    ShortenContext dbContext,
    CancellationToken cancellationToken) =>
{
    var shortenedUrl = string.Empty;
    if (request.CustomShortenedUrl is null)
    {
        shortenedUrl = Utils.GenerateUrl();
    }
    else
    {
        var exists = await dbContext
            .Shorten
            .FirstOrDefaultAsync(shortenedUrl => shortenedUrl.Id == request.CustomShortenedUrl, cancellationToken);

        if (exists is not null)
        {
            return Results.Conflict("This shortened URL already exists!");
        }

        shortenedUrl = request.CustomShortenedUrl;
    }

    var urlItem = new UrlItem
    {
        Id = shortenedUrl,
        ShortenedUrl = $"{url}/{shortenedUrl}",
        MappedUrl = request.Url,
        RedirectCount = 0
    };

    await dbContext.AddAsync(urlItem, cancellationToken);
    await dbContext.SaveChangesAsync(cancellationToken);

    return Results.Ok(new UrlResult { Url = $"{url}/{shortenedUrl}" });
});

app.MapGet("/{shortenedUrl}", async (string shortenedUrl, ShortenContext dbContext) =>
{
    var exists = await dbContext.Shorten.FirstOrDefaultAsync(urlItem => urlItem.Id == shortenedUrl);
    if (exists is null) return Results.NotFound("There is no mapping for this URL");

    exists.RedirectCount++;

    await dbContext.SaveChangesAsync();

    return Results.Redirect(exists.MappedUrl);
});

app.Run();
