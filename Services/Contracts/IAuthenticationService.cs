using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Identity;

namespace Services.Contracts;
public interface IAuthenticationService {
    Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistrationDto);
    Task<Boolean> ValidateUser(UserForAuthenticationDto userForAuthDto);
    Task<TokenDto> CreateToken(Boolean populateExp);
    Task<TokenDto> RefreshToken(TokenDto tokenDto);
}