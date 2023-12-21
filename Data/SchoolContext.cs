using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;
using System.Collections.Generic; 
using System.Threading.Tasks;

namespace ContosoUniversity.Data
{
    public class SchoolContext : DbContext
    {
        public SchoolContext(DbContextOptions<SchoolContext> options)
            : base(options)
        {
        }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Category> Categorys { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Employee> Employees { get; set; }

        public DbSet<Leaverequest> Leaverequests { get; set; }     
        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Course> Courses { get; set; }

        public async Task<List<Status>> GetStatusesAsync()
        {
            return await Statuses.ToListAsync();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>().ToTable("Course");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<Role>().ToTable("Role");
            modelBuilder.Entity<Team>().ToTable("Team");   
            modelBuilder.Entity<Status>().ToTable("status");
            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<Employee>().ToTable("Employee");
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Leaverequest>().ToTable("Leaverequest");
        }

        public DbSet<ContosoUniversity.Models.Leaverequest> Leaverequest { get; set; }

        public DbSet<ContosoUniversity.Models.Employee> Employee { get; set; }
    }
}