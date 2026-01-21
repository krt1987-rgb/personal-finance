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
    private readonly IRepository<RefreshToken> _refreshTokenRepository;
    private readonly IRepository<PasswordResetToken> _resetTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public AuthService(
        IRepository<User> userRepository,
        IRepository<RefreshToken> refreshTokenRepository,
        IRepository<PasswordResetToken> resetTokenRepository,
        IUnitOfWork unitOfWork,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _resetTokenRepository = resetTokenRepository;
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
        var refreshToken = await CreateRefreshTokenAsync(user.Id);
        
        return new LoginResponseDto
        {
            Token = token,
            RefreshToken = refreshToken.Token,
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
        var refreshToken = await CreateRefreshTokenAsync(user.Id);
        
        return new LoginResponseDto
        {
            Token = token,
            RefreshToken = refreshToken.Token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(GetTokenExpirationMinutes()),
            User = MapToUserDto(user)
        };
    }

    public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
    {
        // Find and validate the refresh token
        var refreshTokens = await _refreshTokenRepository.FindAsync(rt => rt.Token == refreshTokenDto.RefreshToken);
        var refreshToken = refreshTokens.FirstOrDefault();
        
        if (refreshToken == null || !refreshToken.IsActive)
            throw new UnauthorizedAccessException("Invalid or expired refresh token");

        var user = await _userRepository.GetByIdAsync(refreshToken.UserId);
        if (user == null || !user.IsActive)
            throw new UnauthorizedAccessException("User not found or inactive");

        // Revoke old token
        refreshToken.IsRevoked = true;
        refreshToken.RevokedAt = DateTime.UtcNow;
        refreshToken.RevokedReason = "Replaced by new token";
        
        var newToken = GenerateJwtToken(user);
        var newRefreshToken = await CreateRefreshTokenAsync(user.Id);
        
        // Store replacement token reference
        refreshToken.ReplacedByToken = newRefreshToken.Token;
        await _refreshTokenRepository.UpdateAsync(refreshToken);
        await _unitOfWork.SaveChangesAsync();
        
        return new LoginResponseDto
        {
            Token = newToken,
            RefreshToken = newRefreshToken.Token,
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

        // Generate reset token
        var resetToken = await CreatePasswordResetTokenAsync(user.Id);
        
        // TODO: Send email with reset link
        // For now, log the token (in production, email it)
        // Example: https://yourapp.com/reset-password?token={resetToken.Token}&email={user.Email}
        
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

        // Find and validate reset token
        var resetTokens = await _resetTokenRepository.FindAsync(rt => 
            rt.Token == resetPasswordDto.Token && rt.UserId == user.Id);
        var resetToken = resetTokens.FirstOrDefault();
        
        if (resetToken == null || !resetToken.IsValid)
            throw new InvalidOperationException("Invalid or expired reset token");
        
        // Update password
        user.PasswordHash = HashPassword(resetPasswordDto.NewPassword);
        
        // Mark token as used
        resetToken.IsUsed = true;
        resetToken.UsedAt = DateTime.UtcNow;
        
        await _userRepository.UpdateAsync(user);
        await _resetTokenRepository.UpdateAsync(resetToken);
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

    private async Task<RefreshToken> CreateRefreshTokenAsync(Guid userId)
    {
        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(7), // Refresh token valid for 7 days
            IsRevoked = false
        };

        await _refreshTokenRepository.AddAsync(refreshToken);
        await _unitOfWork.SaveChangesAsync();
        
        return refreshToken;
    }

    private async Task<PasswordResetToken> CreatePasswordResetTokenAsync(Guid userId)
    {
        var resetToken = new PasswordResetToken
        {
            UserId = userId,
            Token = GeneratePasswordResetToken(),
            ExpiresAt = DateTime.UtcNow.AddHours(1), // Reset token valid for 1 hour
            IsUsed = false
        };

        await _resetTokenRepository.AddAsync(resetToken);
        await _unitOfWork.SaveChangesAsync();
        
        return resetToken;
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

    private string GeneratePasswordResetToken()
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
