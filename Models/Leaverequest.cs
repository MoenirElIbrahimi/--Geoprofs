namespace ContosoUniversity.Models
{
    public class Leaverequest
    {
        public int ID { get; set; }
        public string Reason { get; set; }

        public LeaveType Type { get; set; }

        public Status Status { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Employee Employee { get; set; }

        public string Remark { get; set; }

        public enum LeaveType
        {
            Ziek,
            VrijeDag,
            vakantie,
            speciaal,
            // Voeg andere leave types toe zoals nodig
        }
    }
}

