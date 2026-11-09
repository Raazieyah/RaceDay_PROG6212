using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http; //7.1 add organiser 6.1

namespace RaceDay.Mvc_.Models
{ 
   
    public class EventDetailsVm
    {
        public EventDto Event { get; set; } = new();
        public List<CategoryDto> Categories { get; set; } = new();
    }
   
    public class ParticipantEnrolmentDto
    {
        public int EnrolmentId { get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
    }

    public class ResultDto
    {
        public int Id { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public TimeSpan FinishTime { get; set; }
        public int FinishingPosition { get; set; }
    }

    public class ParticipantProfileVm
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public string? ProfilePictureUrl { get; set; }

        public IFormFile? ProfilePicture { get; set; }
    }
}
