namespace RatingApp.Models
{
    public class LoginRequest
    {
        public string username { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string access { get; set; } = string.Empty;
        public string refresh { get; set; } = string.Empty;
        public int id { get; set; }
        public string guid { get; set; } = string.Empty;
    }

    public class AuthInfo
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public int UserId { get; set; } 
        public string Username { get; set; } = string.Empty;
        public bool IsAuthenticated => !string.IsNullOrEmpty(Token) && ExpiresAt > DateTime.Now;
    }
}