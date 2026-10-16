using System;

namespace Events.Models
{
	public Events
	{//Events ERD attributes
        //my primary key
       public int EventID {  get; set; }

       public string EventName {  get; set; } = string.Empty
       public string Location {  get; set; } = string.Empty
       public string description {  get; set; } = string.Empty
       public DateTime EventDate {  get; set; }//double check if int or string
       public string EventName { get; set; }
       public double Distance {  get; set; }
    //Foreign keys
       public int OrganiserID {  get; set; }
       public int CategoryID {  get; set; }


    public Organiser Organiser { get; set; } = null!;
    public Category Category { get; set; } = null!;
}
}

