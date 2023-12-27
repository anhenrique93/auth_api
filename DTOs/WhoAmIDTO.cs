using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskCircle.AuthentcationApi.DTOs;

public class WhoAmIDTO
{
    public int IdUser { get; set; }

    public string? Email { get; set; }
}
