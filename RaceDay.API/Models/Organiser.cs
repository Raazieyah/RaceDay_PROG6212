using System;
using System.Collections.Generic;

namespace Organiser.Models
{//organiseer

    public class Organiser
	{
        public int OrganiserID { get; set; }//primary key
        
        public string Name { get; set; } =string.Empty
        //public int EventID { get; set; }
        public string Email { get; set; } = string.Empty

            //navigator
            public ICollection<Events.Models.Event> Events { get; set; } = new List<Events.Models.Event>();
    }
}
