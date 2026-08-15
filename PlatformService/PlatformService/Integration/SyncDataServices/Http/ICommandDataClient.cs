using PlatformService.Dto;

namespace PlatformService.Integration.SyncDataServices.Http;

public interface ICommandDataClient
{
    Task SendPlatformToCommandService(PlatformResponseDto platform);
}