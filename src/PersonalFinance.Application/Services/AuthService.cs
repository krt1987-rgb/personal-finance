using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Domain.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PersonalFinance.Application.Services;

public class AuthService : IAuthService
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public AuthService(
        IRepository<User> userRepository,
        IUnitOfWork unitOfWork,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
    {
        var users = await _userRepository.FindAsync(u => u.Email == loginDto.Email);
        var user = users.FirstOrDefault();
        
        if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Account is deactivated");

        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();
        
        return new LoginResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(GetTokenExpirationMinutes()),
            User = MapToUserDto(user)
        };
    }

    public async Task<LoginResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        if (registerDto.Password != registerDto.ConfirmPassword)
            throw new ArgumentException("Passwords do not match");

        var existingUsers = await _userRepository.FindAsync(u => u.Email == registerDto.Email);
        if (existingUsers.Any())
            throw new InvalidOperationException("Email already registered");

        var user = new User
        {
            Email = registerDto.Email,
            PasswordHash = HashPassword(registerDto.Password),
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            PhoneNumber = registerDto.PhoneNumber,
            DateOfBirth = registerDto.DateOfBirth,
            IsActive = true
        };

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();
        
        return new LoginResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(GetTokenExpirationMinutes()),
            User = MapToUserDto(user)
        };
    }

    public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
    {
        // TODO: Implement refresh token validation and storage
        // For now, validate the old token and generate a new one
        var principal = ValidateToken(refreshTokenDto.Token);
        var userId = Guid.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
        
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || !user.IsActive)
            throw new UnauthorizedAccessException("Invalid token");

        var newToken = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();
        
        return new LoginResponseDto
        {
            Token = newToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(GetTokenExpirationMinutes()),
            User = MapToUserDto(user)
        };
    }

    public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
    {
        var users = await _userRepository.FindAsync(u => u.Email == forgotPasswordDto.Email);
        var user = users.FirstOrDefault();
        
        if (user == null)
            return false; // Don't reveal if email exists

        // TODO: Generate reset token and send email
        // For now, just return success
        // In production: Store reset token with expiration, send email with link
        
        return true;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmPassword)
            throw new ArgumentException("Passwords do not match");

        var users = await _userRepository.FindAsync(u => u.Email == resetPasswordDto.Email);
        var user = users.FirstOrDefault();
        
        if (user == null)
            throw new InvalidOperationException("Invalid reset token");

        // TODO: Validate reset token
        // For now, just update the password
        
        user.PasswordHash = HashPassword(resetPasswordDto.NewPassword);
        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto)
    {
        if (changePasswordDto.NewPassword != changePasswordDto.ConfirmPassword)
            throw new ArgumentException("Passwords do not match");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        if (!VerifyPassword(changePasswordDto.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedAccessException("Current password is incorrect");

        user.PasswordHash = HashPassword(changePasswordDto.NewPassword);
        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
        
        return true;
    }

    public Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            ValidateToken(token);
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT secret not configured");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim("firstName", user.FirstName),
            new Claim("lastName", user.LastName)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(GetTokenExpirationMinutes()),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private ClaimsPrincipal ValidateToken(string token)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT secret not configured");
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false, // Don't validate lifetime for refresh
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };

        return tokenHandler.ValidateToken(token, validationParameters, out _);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private string HashPassword(string password)
    {
        // Using BCrypt-like hashing (simplified for demo - use BCrypt.Net in production)
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "SALT"));
        return Convert.ToBase64String(hashedBytes);
    }

    private bool VerifyPassword(string password, string passwordHash)
    {
        var hash = HashPassword(password);
        return hash == passwordHash;
    }

    private int GetTokenExpirationMinutes()
    {
        var minutes = _configuration.GetSection("JwtSettings")["ExpirationInMinutes"];
        return int.TryParse(minutes, out var result) ? result : 60;
    }

    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            DateOfBirth = user.DateOfBirth,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}
