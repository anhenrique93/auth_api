
namespace TaskCircle.AuthentcationApi.Models;

/// <summary>
/// This class represents a user in the application, including its fields in the User table.
/// </summary>
public class User
{
    /// <summary>
    /// User Id
    /// </summary>
    public int IdUser { get; set; }
    /// <summary>
    /// User Email
    /// </summary>
    /// <example>nome@exemple.com</example>
    public string? Email { get; set; }
    /// <summary>
    /// User Password with hash
    /// </summary>
    public byte[]? PasswordHash { get; set; }
    /// <summary>
    /// Password hash salt
    /// </summary>
    public byte[]? PasswordSalt { get; set; }
    /// <summary>
    /// "Refresh token to extend token expiration.
    /// </summary>
    public string? RefreshToken { get; set; }
    /// <summary>
    /// Creation date of the refresh token.
    /// </summary>
    /// <example>2023-12-21 12:17:53.040</example>
    public DateTime TokenCreated { get; set; }
    /// <summary>
    /// Expiration date of the refresh token.
    /// </summary>
    /// <example>2023-12-28 12:17:53.040</example>
    public DateTime TokenExpires { get; set; }

}
