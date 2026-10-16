using System;
using System.Collections.Generic;

namespace Categorys.Models
{//Catergorys ERD attributes
	public Categorys
	{
        public int CategoryID {  get; set; }//primary key
        public string CatergoryType {  get; set; } = string.Empty;
    //public int EventID { get; set; }

    public ICollection<Events.Models.Event> Events { get; set; } = new List<Events.Models.Event>();
}
}
