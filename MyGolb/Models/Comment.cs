using System.Text.Json.Serialization;
using MyGolb.Enums;

namespace MyGolb.Models;

public class Comment
{
    public long Id { get; set; }
    public string? CommetPath { get; set; }
    public CommentType Type { get; set; }
    public DateTime Date { get; set; }
    public User User { get; set; }
    public Post Post { get; set; }
    [JsonIgnore]
    public virtual List<Interaction>? Interactions { get; set; }
}