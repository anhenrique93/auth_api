
namespace TaskCircle.AuthentcationApi.Models;

public class User
{
    public int IdUser { get; set; }
    public string? Email { get; set; }
    public byte[]? PasswordHash { get; set; }
    public byte[]? PasswordSalt { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime TokenCreated { get; set; }
    public DateTime TokenExpires { get; set; }

}
