using Microsoft.EntityFrameworkCore;


public class ApplicationContext : DbContext
{
    public DbSet<MyTask> MyTasks { get; set; } = null!;

    public ApplicationContext()
    {
        Database.EnsureCreated();
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=TaskManager;Username=postgres;Password=qwerty");
    }

}
