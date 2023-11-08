namespace ContosoUniversity.Models
{
    public class User
    {
        public int ID { get; set; }
        public Employee Employee { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
