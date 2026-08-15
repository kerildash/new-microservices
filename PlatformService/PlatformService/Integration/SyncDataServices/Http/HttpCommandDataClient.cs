using System.Text;
using System.Text.Json;
using PlatformService.Dto;

namespace PlatformService.Integration.SyncDataServices.Http;

public class HttpCommandDataClient(HttpClient httpClient) : ICommandDataClient
{
    public async Task SendPlatformToCommandService(PlatformResponseDto platform)
    {
        var httpContent = new StringContent(
            JsonSerializer.Serialize(platform),
            Encoding.UTF8,
            "application/json");

        var response = await httpClient.GetAsync("http://commands-clusterip:80/api/c/command");
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Platform sent successfully");
            Console.WriteLine($"Platform response: {await response.Content.ReadAsStringAsync()}");
            return;
        }

        Console.WriteLine($"Error while sending a platform. {response}");
    }
}