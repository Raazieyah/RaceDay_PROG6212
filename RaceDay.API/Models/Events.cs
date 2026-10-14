using System;

namespace Events.Models
{
	public Events
	{//Events ERD attributes

       public int EventID {  get; set; }
       public string EventName {  get; set; }
       public string Location {  get; set; }
       public string description {  get; set; }
       public string EventDate {  get; set; }//double check if int or string
       public string EventName { get; set; }
       public double Distance {  get; set; }
       public int OrganiserID {  get; set; }
       public int CatergoryID {  get; set; }
}
}

