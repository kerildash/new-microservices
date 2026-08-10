namespace CommandService.Models;

public class Platform
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<Command> Commands { get; set; } = [];
}