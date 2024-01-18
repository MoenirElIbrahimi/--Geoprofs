using ContosoUniversity.Models;
using Microsoft.AspNetCore.Identity;
using System.Drawing;
using static ContosoUniversity.Models.Leaverequest;



namespace ContosoUniversity.Data
{
    public static class DbInitializer
    {
        public static void Initialize(SchoolContext context)
        {
          
            var teams = new Team[]
            {
                new Team{Name="TeamA"},
                new Team{Name="TeamB"},
            };
          
            context.Teams.AddRange(teams);
            context.SaveChanges();



            var roles = new Role[]
            {
                new Role{Name="Manager"},
                new Role{Name="Employee"},
            };
          
            context.Roles.AddRange(roles);
            context.SaveChanges();
          
            // Seed Employees and Users
            if (context.Employees.Any())
            {
                return;   // DB has been seeded
            }
          
            var employees = new Employee[]
            {
                new Employee{FirstName="Manager",LastName="Manager", Role=roles[0], Team=teams[0],StartDate=DateTime.Parse("2020-01-01"),VacationDays=20},
                new Employee{FirstName="Werknemer",LastName="Werknemer", Role=roles[1], Team=teams[0],StartDate=DateTime.Parse("2020-01-01"),VacationDays=40},
                new Employee{FirstName="Manager2",LastName="Manager2", Role=roles[0], Team=teams[1],StartDate=DateTime.Parse("2020-01-01"),VacationDays=30},
                new Employee{FirstName="Werknemer2",LastName="Werknemer2", Role=roles[1], Team=teams[0],StartDate=DateTime.Parse("2020-01-01"),VacationDays=10},
                new Employee{FirstName="Werknemer3",LastName="Werknemer3", Role=roles[1], Team=teams[1],StartDate=DateTime.Parse("2020-01-01"),VacationDays=15},
            };
          
            context.Employees.AddRange(employees);
            context.SaveChanges();
          
            var users = new User[]
            {
                new User{Employee=employees[0], Email="manager@geoprofs.com", Password="manager"},
                new User{Employee=employees[1], Email="werknemer@geoprofs.com", Password="werknemer"},
                new User{Employee=employees[2], Email="manager2@geoprofs.com", Password="manager2"},
                new User{Employee=employees[3], Email="werknemer2@geoprofs.com", Password="werknemer2"},
                new User{Employee=employees[4], Email="werknemer3@geoprofs.com", Password="werknemer3"},
            };
          
            context.Users.AddRange(users);
            context.SaveChanges();
          
            // Assign manager to employee
            employees[1].Manager = employees[0];
            employees[3].Manager = employees[2];
            employees[4].Manager = employees[0];
            context.SaveChanges();
          
            var categories = new Category[]
            {
                new Category{Name="Vacation"},
                new Category{Name="Personal"},
                new Category{Name="Sick"},
            };
          
            context.Categorys.AddRange(categories);
            context.SaveChanges();
          
            var statuses = new Status[]
            {
                new Status{Name="Requested", Color="#63A2E2"},
                new Status{Name="Accepted", Color="#46C298"},
                new Status{Name="Denied", Color="#E64A2C"},
            };
          
            context.Statuses.AddRange(statuses);
            context.SaveChanges();
          
            // Seed leave requests
            foreach (var employee in employees)
            {
                for (int i = 0; i < 15; i++)
                {
                    var leaveRequest = new Leaverequest
                    {
                        Reason = "Vacation",
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddHours(1),
                        Status = statuses[i % 3],
                        Employee = employee,
                        Category = categories[i % 3],
                        CreatedAt = DateTime.Now
                    };
                    context.Leaverequests.Add(leaveRequest);
                }
            }
          
            context.SaveChanges();
        }
    }
}