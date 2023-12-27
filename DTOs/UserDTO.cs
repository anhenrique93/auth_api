using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace TaskCircle.AuthentcationApi.DTOs;

public class UserDTO
{

    public int IdUser { get; set; }

    [Required(ErrorMessage = "The email is Required")]
    [EmailAddress(ErrorMessage = "It has to be an email")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "The password is Required")]
    [PasswordPropertyText]
    [MinLength(6, ErrorMessage = "The password must have at least 6 characters.")]
    [RegularExpression(@"^(?=.*[0-9])(?=.*[!@#$%^&*])[a-zA-Z0-9!@#$%^&*]+$",
        ErrorMessage = "The password must contain at least one number and one special character.")]
    public string? Password { get; set; }

    [JsonIgnore]
    [XmlIgnore]
    public byte[]? PasswordHash { get; set; }

    [JsonIgnore]
    [XmlIgnore]
    public byte[]? PasswordSalt { get; set; }

    [JsonIgnore]
    [XmlIgnore]
    public string? RefreshToken { get; set; }

    [JsonIgnore]
    [XmlIgnore]
    public DateTime TokenCreated { get; set; }

    [JsonIgnore]
    [XmlIgnore]
    public DateTime TokenExpires { get; set; }
}
