using System;
using System.ComponentModel.DataAnnotations;//double check

namespace RaceDay.Contracts
{
    //DTOS inclucing model validation
    //DTOs for authentication
    public record LoginRequest(
        [Required]
        [Email adress] string Email,
        [Required]
        [MinLength(8)] string Password
        );
    

    public record RegisterRequest(
        [Required]
        [StringLength(50)] string FirstName,
        [Required]
        [StringLength(50)] string LastName,
        [Required]
        [Email adress] string Email, //string Password?, 
        [Required]
        string Role
        );

    public record AuthResponse(
        string Token,
        string Id,
        string Email,
        string Role
        );

    //DTOs for profiles
    public record ProfileDto(
        string FirstName,
        string LastName,
        string Email, //string Password?,
        string Role
        );

    public record UpdateProfileRequest(
        [Required]
        [StringLength(50)]string FirstName,
        [Required]
        [StringLength(50)]string LastName,
        [Required]
        [EmailAddress]string Email
        );

    //DTOs for Events
    public record EventDto(
        int Id,
        string Name,
        string Description,
        DateTime Date,
        String Location,
        int CategoryId
        );
    public record UpdateEventRequest(
        [Required]
        [StringLength(100)]string Name,
        [Required]
        [StringLength(600)]string Description,
        [Required]
        DateTime Date,
        [Required]
        [StringLength(200)]string Location,
        int CatergoryId
        );

    //Enrolment DTOs
    public record EnrolmentDto(
        int Id,
        int EventId,
        string UserId,
        string Status,
        DateTime EnrolledAt
        );

    public record CreateEnrolmentRequest(
        [Required]
        [Range(1, int.MaxValue)] int EventId
        );
    public record UpdateEnrolmentStatusRequest(
        [Required]
        [StringLength(50)] string Status
        );
    //DTOS For events
    public record CategoryDto(
        int Id,
        string Name,
        string Description
        );

    public record CreateCategoryRequest(
        [Required]
        [StringLength(100)] string Name,
        [Required]
        [StringLength(600)] string Description
        );

    //results dtos
    public record ResultDto(
        int Id,
        int EventId,
        string UserId,
        string Position,
        string TimeTaken
        );

    public record CreateResultsRequest(
        [Required]
        [Range(1, int.MaxValue)] int EventId
        [Required]
        string UserId,
        [Required]
        [StringLength(50)] string Position,
        [Required]
        [StringLength(50)] string TimeTaken
        );

    //image upload(part3)
    public record UploadResponse(
        string FileUrl,
        string Filename,
        bool IsSuccess
        );
}
