using Serene.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

namespace Serene.Data;
public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<JournalEntry> JournalEntries { get; set; } = null!;

    private readonly string _dbPath;

    public AppDbContext()
    {
        // Path to store SQLite DB on device
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _dbPath = System.IO.Path.Combine(folder, "serene_database.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={_dbPath}");
    }
}