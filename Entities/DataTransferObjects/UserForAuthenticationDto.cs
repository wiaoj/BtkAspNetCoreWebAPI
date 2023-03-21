using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObjects;
public record UserForAuthenticationDto([Required(ErrorMessage = "Username is required.")] String? UserName,
                                       [Required(ErrorMessage = "Password is required.")] String? Password);