namespace BlogCore.DAL.Data;

using BlogCore.DAL.Models;
using Microsoft.EntityFrameworkCore;

public class BlogContext : DbContext
{
    // Konstruktor pozwalaj¹cy na wstrzykniêcie konfiguracji (np. z Testcontainers) 
    public BlogContext(DbContextOptions<BlogContext> options) : base(options)
    {
    }

    // Definicje tabel w bazie danych 
    public DbSet<Post> Posts { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            entity.Property(p => p.Author)
                .IsRequired();

            entity.Property(p => p.Content)
                .IsRequired();

            entity.HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            entity.Property(c => c.Content)
                .IsRequired();
        });
    }
}

