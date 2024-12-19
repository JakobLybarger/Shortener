using System.Threading.Channels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Shorten.Data;
using Shorten.Data.Data;
using Shortener;
using Shortener.Models;
using Shortener.Utils;

var builder = WebApplication.CreateBuilder(args);

var connectionStringEnv = builder.Configuration.GetConnectionString("Shorten");
if (connectionStringEnv is null)
    throw new Exception("EnvironmentVariable 'ConnectionString' is required.");

var connectionString = Environment.GetEnvironmentVariable(connectionStringEnv);

builder.Services.AddDbContext<ShortenContext>(options => options.UseSqlServer(connectionString));

// Add Cache
builder.Services.AddMemoryCache();

// Add Channel
builder.Services.AddSingleton(Channel.CreateUnbounded<string>());

// Add Background service
builder.Services.AddHostedService<RedirectUpdater>();

var app = builder.Build();

const string url = "http://localhost:5249";
var mappings = new Dictionary<string, string>();

app.MapPost("/shorten", async (
    [FromBody] ShortenRequest request,
    ShortenContext dbContext,
    IMemoryCache cache,
    CancellationToken cancellationToken) =>
{
    string shortenedUrl;
    if (request.CustomShortenedUrl is null)
    {
        shortenedUrl = Utils.GenerateUrl();
    }
    else
    {
        var exists = await dbContext
            .Shorten
            .FirstOrDefaultAsync(u => u.Id == request.CustomShortenedUrl, cancellationToken);

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

    cache.Set(shortenedUrl, request.Url, TimeSpan.FromMinutes(5));

    await dbContext.AddAsync(urlItem, cancellationToken);
    await dbContext.SaveChangesAsync(cancellationToken);

    return Results.Ok(new UrlResult { Url = $"{url}/{shortenedUrl}" });
});

app.MapGet("/{shortenedUrl}", async (
    string shortenedUrl,
    ShortenContext dbContext,
    IMemoryCache cache,
    Channel<string> channel,
    CancellationToken cancellationToken) =>
{
    var longUrl = string.Empty;
    if (cache.TryGetValue(shortenedUrl, out string? originalUrl))
    {
        longUrl = originalUrl ?? string.Empty;
    }
    else
    {
        var exists = await dbContext.Shorten.FirstOrDefaultAsync(urlItem => urlItem.Id == shortenedUrl, cancellationToken);
        if (exists is null) return Results.NotFound("There is no mapping for this URL");
        longUrl = exists.MappedUrl;

        cache.Set(shortenedUrl, longUrl, TimeSpan.FromMinutes(5));
    }

    var writer = channel.Writer;
    await writer.WriteAsync(shortenedUrl, cancellationToken);

    return Results.Redirect(longUrl);
});

app.Run();
