namespace ContosoUniversity.Models
{
    public class Employee
    {
        public int ID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }

        public int Bsn { get; set; }

        public Team Team { get; set; }

        public DateTime StartDate { get; set; }
        public Role Role { get; set; }

        public Employee Manager { get; set; }

        public int VacationDays { get; set; }

    }
}