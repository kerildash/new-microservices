using CommandService.Dto;
using CommandService.Models;

namespace CommandService.Mapping;

public static class CommandMappingExtensions
{
    extension(Command command)
    {
        public static Command FromCreateDto(CommandCreateDto dto) => new()
        {
            Line = dto.Line,
            Description = dto.Description
        };

        public CommandResponseDto ToResponseDto() => new()
        {
            Line = command.Line,
            Description = command.Description,
            Id = command.Id
        };
    }
}