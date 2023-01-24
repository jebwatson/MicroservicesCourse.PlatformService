using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http;

public class HttpCommandDataClient : ICommandDataClient
{
  private readonly HttpClient httpClient;
  private readonly IConfiguration config;

  public HttpCommandDataClient(HttpClient httpClient, IConfiguration config)
  {
    this.httpClient = httpClient;
    this.config = config;
  }

  public async Task SendPlatformToCommand(PlatformReadDto platform)
  {
    var httpContent = new StringContent(
        JsonSerializer.Serialize(platform),
        Encoding.UTF8,
        "application/json"
    );

    var response = await httpClient.PostAsync(config["CommandService"], httpContent);

    if (response.IsSuccessStatusCode)
    {
      System.Console.WriteLine("--> Sync POST to CommandService was OK!");
    }
    else
    {
      System.Console.WriteLine("--> Sync POST to CommandService was not OK!");
    }
  }
}
