using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Models
{
    public class Leaverequest
    {
        public int ID { get; set; }
        public string Reason { get; set; }

        public Category Category { get; set; }

        public Status Status { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        public Employee Employee { get; set; }

        public string Remark { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

