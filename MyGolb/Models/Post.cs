using System.Text.Json.Serialization;
using MyGolb.Enums;

namespace MyGolb.Models;

public class Post
{
    public long Id { get; set; }
    public string? PostPath { get; set; }
    public PostType Type { get; set; }
    public DateTime Date { get; set; }
    public User User { get; set; }
    [JsonIgnore]
    public virtual List<Comment>? Comments { get; set; }
    [JsonIgnore]
    public virtual List<Interaction>? Interactions { get; set; }
}