namespace CommandService.Models;

public class Command
{
    public int Id { get; set; }
    public required string Description { get; set; }
    public required string Line { get; set; }
    public int PlatformId { get; set; }
    public Platform Platform { get; set; } = null!;
}