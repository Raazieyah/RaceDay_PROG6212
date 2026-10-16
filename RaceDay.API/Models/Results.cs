using System;

namespace Results.Models //Erd atrributes
{
	public class Result
	{
    public int ResultID { get; set; }//PRIMARY KEY

    public string FinishTime { get; set; } = string.Empty;
    public int FinishPosition { get; set; }

        //foreign keys
   // public int ParticipantID { get; set; }
    public string CategoryType { get; set; }
    public int EventID { get; set; }

        //nav properties
        public Event Event { get; set; } = null!;
        public Participant.Models.Participant Participant { get; set; } = null!;
    }
}
