namespace ContosoUniversity.Models
{
    public class Employee
    {
        public int ID { get; set; }
        public string LastName { get; set; }
        public string FirstMidName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Team { get; set; }

        public int Role { get; set; }
    }
}