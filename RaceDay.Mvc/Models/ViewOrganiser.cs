using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


    // 6.1 Organiser Dashboard
    
    namespace RaceDay.Mvc.Models
    {
        
        public class OrganiserDashboardVm
        {
            public List<OrganiserEventSummaryVm> Events { get; set; } = new();
        }

        public class OrganiserEventSummaryVm
        {
            public int EventId { get; set; }
            public string Name { get; set; } = string.Empty;
            public DateTime EventDate { get; set; }
            public int EnrolmentCount { get; set; }
        }
    }


public class EventDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public double DistanceKm { get; set; }
    }

    public class CreateEventRequest
    {
        [Required] public string Name { get; set; } = string.Empty;
        [Required] public string Description { get; set; } = string.Empty;
        [Required] public DateTime EventDate { get; set; } = DateTime.Today.AddDays(7);
        [Required] public string Location { get; set; } = string.Empty;
        [Required] public double DistanceKm { get; set; }
        
    }

    public class UpdateEventRequest : CreateEventRequest
    {
        public int Id { get; set; }
    }

    
    
   // 6.4 Categories - same as DTOs
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty; 
    }


public class CreateCategoryRequest
    {
        public int EventId { get; set; }
        [Required] public string Name { get; set; } = string.Empty;
    }

    // 6.5 Enrolments
    public class OrganiserEnrolmentVm
    {
        public int EnrolmentId { get; set; }
        public int EventId { get; set; }
        public string ParticipantName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public bool HasResult { get; set; }
    }
    public class CaptureResultVm
    {
        public int EnrolmentId { get; set; }
        [Required] public TimeSpan FinishTime { get; set; }
        [Required] public int FinishingPosition { get; set; }
    }
}

