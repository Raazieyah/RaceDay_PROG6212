using System;

namespace Enrolment.Models
{
    //Enrolment erd attributes
    public class Enrolement
	{
        public int EnrolmentID { get; set; }

        public int EventID { get; set; }
        //public int CatergoryID { get; set; }
        public int ParticipantID { get; set; }

        public DateTime EnrolledAt { get; set; } = DateTime.Now;

        public Event Event { get; set; } = null!;
        public Participant.Models.Participant Participant { get; set; } = null!;//nav properties

    }
}
