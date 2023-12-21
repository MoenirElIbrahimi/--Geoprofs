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

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await Categorys.ToListAsync();
        }

        public async Task<List<Status>> GetStatusesAsync()
        {
            return await Statuses.ToListAsync();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>().ToTable("Course");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<Leaverequest>().ToTable("Leaverequest");
            modelBuilder.Entity<Employee>().ToTable("Employee");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<Team>().ToTable("Teams");   
            modelBuilder.Entity<Status>().ToTable("statuses");
            modelBuilder.Entity<Category>().ToTable("Categorys");
            modelBuilder.Entity<User>().ToTable("Users");
        }

        public DbSet<ContosoUniversity.Models.Leaverequest> Leaverequest { get; set; }

        public DbSet<ContosoUniversity.Models.Employee> Employee { get; set; }
    }
}