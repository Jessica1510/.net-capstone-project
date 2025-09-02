namespace Account_microservice.Models.DTO
{
    public class AccountDtos
    {
        public record RegisterDto(string Email, string Password, string? FullName);
        public record LoginDto(string Email, string Password);
        public record AuthResponse(string Token, DateTime ExpiresAt, string UserId, string Email, string[] Roles);
    }
}
