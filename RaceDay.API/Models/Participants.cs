using System;
using System.Collections.Generic;

namespace Participant.Models
{
    //participants erd attributes
    public class Participant
	{
        //primary key
    public int ParticipantID { get; set; }

    public string Name { get; set; } = string.Empty;
        public DateTime PhoneNumber { get; set; } = string.Empty;
    public string Gender{ get; set; } = string.Empty ;
    public string DOB { get; set; }
        // public int EventID { get; set; }

        //navigator
        public ICollection<RaceDay.Contracts.EnrolmentDto> Enrolments { get; set; } = new List<RaceDay.Contracts.EnrolmentDto>();
    }
}
