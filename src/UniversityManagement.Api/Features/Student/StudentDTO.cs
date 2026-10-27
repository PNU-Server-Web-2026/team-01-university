using System.ComponentModel.DataAnnotations; 

namespace UniversityManagement.Api.Features.Students; 

public class Student 
{
    public int Id { get; set; } 
    public string FirstName { get; set; } = string.Empty; 
    public string LastName { get; set; } = string.Empty; 
    public string Email { get; set; } = string.Empty; 
    public string RecordBookNumber { get; set; } = string.Empty; 
    public DateOnly BirthDate { get; set; } 
}

public record CreateStudentRequest(
    [Required, MaxLength(100)] string FirstName, 
    [Required, MaxLength(100)] string LastName,  
    [Required, EmailAddress] string Email,      
    [Required, RegularExpression(@"^\d{8}$")] string RecordBookNumber, 
    DateOnly BirthDate 
);


public record UpdateStudentRequest(
    [Required, MaxLength(100)] string FirstName, 
    [Required, MaxLength(100)] string LastName,  
    [Required, EmailAddress] string Email,      
    [Required, RegularExpression(@"^\d{8}$")] string RecordBookNumber, 
    DateOnly BirthDate 
);


public record StudentResponse(
    int Id, 
    string FirstName,
    string LastName,  
    string Email,     
    string RecordBookNumber, 
    DateOnly BirthDate 
);