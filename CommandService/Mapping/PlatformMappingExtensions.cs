using CommandService.Dto;
using CommandService.Models;

namespace CommandService.Mapping;

public static class PlatformMappingExtensions
{
    extension(Platform platform)
    {
        public PlatformResponseDto ToResponseDto() => new()
        {
            Id = platform.Id,
            Name = platform.Name
        };

        public static Platform FromReceivedDto(PlatformReceivedDto dto) => new()
        {
            Id = dto.Id,
            Name = dto.Name,
        };
    }
}