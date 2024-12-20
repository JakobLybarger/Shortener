using Microsoft.EntityFrameworkCore;

namespace Shorten.Data.Data;

public class ShortenContext : DbContext
{

    public ShortenContext(DbContextOptions<ShortenContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UrlItem>()
            .ToTable("shorten");

        modelBuilder.Entity<UrlItem>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<UrlItem>()
            .Property(e => e.Id)
            .IsUnicode(false)
            .HasMaxLength(6)
            .IsRequired()
            .HasColumnName("id");

        modelBuilder.Entity<UrlItem>()
            .Property(e => e.ShortenedUrl)
            .IsRequired()
            .HasColumnName("shortened_url");

        modelBuilder.Entity<UrlItem>()
            .Property(e => e.MappedUrl)
            .IsRequired()
            .HasColumnName("mapped_url");

        modelBuilder.Entity<UrlItem>()
            .Property(e => e.RedirectCount)
            .IsRequired()
            .HasColumnName("redirect_count");
    }
    public DbSet<UrlItem> Shorten { get; set; }
}
