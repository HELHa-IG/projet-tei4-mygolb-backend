using MyGolb.Enums;

namespace MyGolb.Models;

public class Interaction
{
    public long Id { get; set; }
    public InteractionType Type { get; set;}
    public DateTime Date { get; set; }
    public User User { get; set; }
    public Post Post { get; set; }
    public Comment Comment { get; set; }
}