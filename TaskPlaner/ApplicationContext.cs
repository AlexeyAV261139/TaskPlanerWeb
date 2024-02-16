using Microsoft.EntityFrameworkCore;


public class ApplicationContext : DbContext
{
    public DbSet<MyTask> MyTasks { get; set; } = null!;

    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }  
}
