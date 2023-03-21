using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Services;
public class AuthenticationManager : IAuthenticationService {
    private readonly ILoggerService logger;
    private readonly IMapper mapper;
    private readonly UserManager<User> userManager;
    private readonly IConfiguration configuration;

    private User? user;

    public AuthenticationManager(ILoggerService logger,
        IMapper mapper,
        UserManager<User> userManager,
        IConfiguration configuration) {
        this.logger = logger;
        this.mapper = mapper;
        this.userManager = userManager;
        this.configuration = configuration;
    }

    public async Task<TokenDto> CreateToken(Boolean populateExp) {
        SigningCredentials signinCredentials = this.GetSignInCredentials();
        List<Claim> claims = await this.GetClaims();
        JwtSecurityToken tokenOptions = this.GenerateTokenOptions(signinCredentials, claims);

        String refreshToken = this.GenerateRefreshToken();
        this.user.RefreshToken = refreshToken;

        if(populateExp)
            this.user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

        await this.userManager.UpdateAsync(this.user);

        String accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        return new(accessToken, refreshToken);
    }

    public async Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistrationDto) {
        User user = this.mapper.Map<User>(userForRegistrationDto);

        IdentityResult result = await this.userManager.CreateAsync(user, userForRegistrationDto.Password);

        if(result.Succeeded)
            await this.userManager.AddToRolesAsync(user, userForRegistrationDto.Roles);

        return result;
    }

    public async Task<Boolean> ValidateUser(UserForAuthenticationDto userForAuthDto) {
        this.user = await this.userManager.FindByNameAsync(userForAuthDto.UserName);
        Boolean result = this.user is not null && await this.userManager.CheckPasswordAsync(this.user, userForAuthDto.Password);

        if(result is false)
            this.logger.LogWarning($"{nameof(ValidateUser)} : Authentication failed. Wrong username or password.");

        return result;
    }

    private SigningCredentials GetSignInCredentials() {
        IConfigurationSection jwtSettings = this.configuration.GetSection("JwtSettings");

        Byte[] key = Encoding.UTF8.GetBytes(jwtSettings["secretKey"]);

        SymmetricSecurityKey secret = new(key);

        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    private async Task<List<Claim>> GetClaims() {
        List<Claim> claims = new() {
            new Claim(ClaimTypes.Name, this.user.UserName)
        };

        IList<String> roles = await this.userManager.GetRolesAsync(this.user);

        foreach(String role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        return claims;
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signinCredentials,
        List<Claim> claims) {
        IConfigurationSection jwtSettings = this.configuration.GetSection("JwtSettings");

        JwtSecurityToken tokenOptions = new(
                issuer: jwtSettings["validIssuer"],
                audience: jwtSettings["validAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expires"])),
                signingCredentials: signinCredentials);

        return tokenOptions;
    }

    private String GenerateRefreshToken() {
        Byte[] randomNumber = new Byte[32];
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(String token) {
        IConfigurationSection jwtSettings = this.configuration.GetSection("JwtSettings");
        String? secretKey = jwtSettings["secretKey"];

        TokenValidationParameters tokenValidationParameters = new() {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["validIssuer"],
            ValidAudience = jwtSettings["validAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };

        JwtSecurityTokenHandler tokenHandler = new();

        ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        return securityToken is not JwtSecurityToken jwtSecurityToken ||
            jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase) is false
            ? throw new SecurityTokenException("Invalid token.")
            : principal;
    }

    public async Task<TokenDto> RefreshToken(TokenDto tokenDto) {
        ClaimsPrincipal principal = this.GetPrincipalFromExpiredToken(tokenDto.AccessToken);
        User? user = await this.userManager.FindByNameAsync(principal.Identity.Name);

        if(user is null ||
            user.RefreshToken != tokenDto.RefreshToken ||
            user.RefreshTokenExpiryTime <= DateTime.Now) {
            throw new RefreshTokenBadRequestException();
        }

        this.user = user;
        return await this.CreateToken(populateExp: false);
    }
}