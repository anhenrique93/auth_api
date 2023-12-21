using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace TaskCircle.AuthentcationApi.DTOs
{
    public class UpdateUserDTO
    {
        [JsonIgnore]
        [XmlIgnore]
        public int IdUser { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}
