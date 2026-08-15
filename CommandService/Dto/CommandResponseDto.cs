namespace CommandService.Dto;

public class CommandResponseDto
{
    public required int Id { get; set; }
    public required string Description { get; set; }
    public required string Line { get; set; }
}