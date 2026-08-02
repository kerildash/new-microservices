using PlatformService.Dto;
using PlatformService.Models;

namespace PlatformService.Extensions;

public static class PlatformExtensions
{
    extension(Platform platform)
    {
        public PlatformResponseDto ToResponseDto() => new PlatformResponseDto()
        {
            Name = platform.Name,
            Publisher = platform.Publisher,
        };

        public static Platform ToPlatform(PlatformCreateDto dto) => new Platform()
        {
            Name = dto.Name,
            Publisher = dto.Publisher,
        };
    }
}