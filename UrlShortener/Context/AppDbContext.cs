using Microsoft.EntityFrameworkCore;
using UrlShortener.Models;

namespace UrlShortener.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options) { }
    public DbSet<Urls> Urls { get; set; }
}
