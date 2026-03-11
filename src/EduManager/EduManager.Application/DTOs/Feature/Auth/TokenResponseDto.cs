namespace EduManager.Application.DTOs.Feature.Auth;

public record TokenResponseDto(
    string AccessToken,
    string RefreshToken,
    string TokenType,
    int ExpiresIn);
