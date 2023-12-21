using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskCircle.AuthentcationApi.DTOs;

public class WhoAmIDTO
{
    [JsonIgnore]
    public int IdUser { get; set; }

    public string? Email { get; set; }
}
