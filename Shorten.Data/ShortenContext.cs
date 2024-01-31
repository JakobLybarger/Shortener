using Microsoft.EntityFrameworkCore;

namespace Shorten.Data.Data;

public class ShortenContext : DbContext
{

    public ShortenContext(DbContextOptions<ShortenContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UrlItem>()
            .ToTable("Shorten");

        modelBuilder.Entity<UrlItem>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<UrlItem>()
            .Property(e => e.Id)
            .IsUnicode(false)
            .HasMaxLength(6)
            .IsRequired();

        modelBuilder.Entity<UrlItem>()
            .Property(e => e.ShortenedUrl)
            .IsRequired();

        modelBuilder.Entity<UrlItem>()
            .Property(e => e.MappedUrl)
            .IsRequired();

        modelBuilder.Entity<UrlItem>()
            .Property(e => e.RedirectCount)
            .IsRequired();
    }
    public DbSet<UrlItem> Shorten { get; set; }
}
