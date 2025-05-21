using Microsoft.EntityFrameworkCore;

public class ScDbContext : DbContext
{
    public ScDbContext(DbContextOptions<ScDbContext> options)
        : base(options) { }
}
