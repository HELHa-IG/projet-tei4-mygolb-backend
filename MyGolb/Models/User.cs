using System.Text.Json.Serialization;
namespace MyGolb.Models;

public class User
{
    public long Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    
    [JsonIgnore]
    public virtual List<Post>? Posts { get; set; }
    [JsonIgnore]
    public virtual List<Comment>? Comments { get; set; }
    [JsonIgnore]
    public virtual List<Interaction>? Interactions { get; set; }
}