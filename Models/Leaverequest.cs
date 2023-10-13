namespace ContosoUniversity.Models
{
    public class Leaverequest
    {
        public int ID { get; set; }
        public string Reason { get; set; }

        public int Status { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Employee Employee { get; set; }
    }
}
