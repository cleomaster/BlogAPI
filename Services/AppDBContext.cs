using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Services;

public class AppDBContext : DbContext
{
    public DbSet<Post> Posts { get; set; }
    public DbSet<User> Users { get; set; }
    
    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
    {
    }
}