namespace RaceDay.Contracts
{
    //DTOs for authentication
    public record LoginRequest(
        string Email,
        string Password
        );
    public record RegisterRequest(
        string FirstName,
        string LastName,
        string Email, string Password, 
        string role

        );
    public record AuthResponse(
        string Token,
        string Id,
        string Email,
        string role
        );

    //DTOs for profiles
    public record ProfileDto(
        string FirstName,
        string LastName,
        string Email, string Password,
        string role
        );

    public record UpdateProfileRequest(
        string FirstName,
        string LastName,
        string Email
        );

    //DTOs for Events
    public recors EventDto(
        int Id,
        string Name,
        string Description,
        DateTime Date,
        Stirng Location,
        int CatergoryId
        );
    public record UpdateEventRequest(
        string Nmae,
        string Description,
        DateTime Date,
        string Location,
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
        int EventId
        );
    public record UpdateEnrolmentStatusRequest(
        string Status
        );
    //DTOS For events
    public record CatergoryDto(
        int Id,
        string Name,
        string Description
        );

    public record CreateCategoryRequest(
        string Name,
        string Descrition
        );

    //results dtos
    public record ResultDtos(
        int Id,
        int EventId,
        string UserId,
        string Position,
        string TimeTaken
        );

    public record CreateResultsRequest(
        int EventId
        string UserId,
        string Position,
        string TimeTaken
        );

    //image upload
    public record UploadResponse(
        string FileUrl,
        string Filename,
        bool IsSuccess
        );
}
