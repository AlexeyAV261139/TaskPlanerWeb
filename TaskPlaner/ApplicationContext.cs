using Microsoft.EntityFrameworkCore;

namespace TaskPlaner
{
    public class ApplicationContext : DbContext
    {
        public DbSet<MyTask> MyTasks { get; set; } = null!;
        public DbSet<Person> Persons { get; set; } = null!;
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
    }
}