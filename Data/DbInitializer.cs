﻿using ContosoUniversity.Models;
using Microsoft.AspNetCore.Identity;
using System.Drawing;
using static ContosoUniversity.Models.Leaverequest;



namespace ContosoUniversity.Data
{
    public static class DbInitializer
    {
        public static void Initialize(SchoolContext context)
        {
            // Look for any students.
            if (context.Students.Any())
            {
                return;   // DB has been seeded
            }



            var students = new Student[]
            {
                new Student{FirstMidName="Carson",LastName="Alexander",EnrollmentDate=DateTime.Parse("2019-09-01")},
                new Student{FirstMidName="Meredith",LastName="Alonso",EnrollmentDate=DateTime.Parse("2017-09-01")},
                new Student{FirstMidName="Arturo",LastName="Anand",EnrollmentDate=DateTime.Parse("2018-09-01")},
                new Student{FirstMidName="Gytis",LastName="Barzdukas",EnrollmentDate=DateTime.Parse("2017-09-01")},
                new Student{FirstMidName="Yan",LastName="Li",EnrollmentDate=DateTime.Parse("2017-09-01")},
                new Student{FirstMidName="Peggy",LastName="Justice",EnrollmentDate=DateTime.Parse("2016-09-01")},
                new Student{FirstMidName="Laura",LastName="Norman",EnrollmentDate=DateTime.Parse("2018-09-01")},
                new Student{FirstMidName="Nino",LastName="Olivetto",EnrollmentDate=DateTime.Parse("2019-09-01")}
            };
          
            context.Students.AddRange(students);
            context.SaveChanges();
          
            var courses = new Course[]
            {
                new Course{CourseID=1050,Title="Chemistry",Credits=3},
                new Course{CourseID=4022,Title="Microeconomics",Credits=3},
                new Course{CourseID=4041,Title="Macroeconomics",Credits=3},
                new Course{CourseID=1045,Title="Calculus",Credits=4},
                new Course{CourseID=3141,Title="Trigonometry",Credits=4},
                new Course{CourseID=2021,Title="Composition",Credits=3},
                new Course{CourseID=2042,Title="Literature",Credits=4}
            };

            context.Courses.AddRange(courses);
            context.SaveChanges();

            var enrollments = new Enrollment[]
            {
                new Enrollment{StudentID=1,CourseID=1050,Grade=Grade.A},
                new Enrollment{StudentID=1,CourseID=4022,Grade=Grade.C},
                new Enrollment{StudentID=1,CourseID=4041,Grade=Grade.B},
                new Enrollment{StudentID=2,CourseID=1045,Grade=Grade.B},
                new Enrollment{StudentID=2,CourseID=3141,Grade=Grade.F},
                new Enrollment{StudentID=2,CourseID=2021,Grade=Grade.F},
                new Enrollment{StudentID=3,CourseID=1050},
                new Enrollment{StudentID=4,CourseID=1050},
                new Enrollment{StudentID=4,CourseID=4022,Grade=Grade.F},
                new Enrollment{StudentID=5,CourseID=4041,Grade=Grade.C},
                new Enrollment{StudentID=6,CourseID=1045},
                new Enrollment{StudentID=7,CourseID=3141,Grade=Grade.A},
            };

            context.Enrollments.AddRange(enrollments);
            context.SaveChanges();
          
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
                for (int i = 0; i < 4; i++)
                {
                    var leaveRequest = new Leaverequest
                    {
                        Reason = "Vacation",
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddHours(1),
                        Status = statuses[i % 3],
                        Employee = employee,
                        Category = categories[i % 3]
                    };
                    context.Leaverequests.Add(leaveRequest);
                }
            }
          
            context.SaveChanges();
        }
    }
}