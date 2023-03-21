using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObjects;
public record UserForRegistrationDto(String? FirstName,
                                     String? LastName,
                                     [Required(ErrorMessage = "Username is required.")] String? UserName,
                                     [Required(ErrorMessage = "Password is required.")] String? Password,
                                     String? Email,
                                     String? PhoneNumber,
                                     ICollection<String>? Roles);