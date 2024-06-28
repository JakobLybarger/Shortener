using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using Shorten.Data.Data;

namespace Shortener;

public class RedirectUpdater : BackgroundService
{
    private readonly Channel<string> _channel;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public RedirectUpdater(IServiceScopeFactory serviceScopeFactory, Channel<string> channel)
    {
        _channel = channel;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var reader = _channel.Reader;

        await foreach (var id in reader.ReadAllAsync(cancellationToken))
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ShortenContext>();

            await dbContext.Shorten
                .Where(item => item.Id == id)
                .ExecuteUpdateAsync(setters => setters.SetProperty(item => item.RedirectCount, item => item.RedirectCount + 1), cancellationToken);
        }
    }
}
