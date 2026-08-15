using PlatformService.Dto;

namespace PlatformService.Integration.Messaging;

public interface IMessageBusClient
{
    Task PublishPlatform(PlatformPublishDto platform);
}