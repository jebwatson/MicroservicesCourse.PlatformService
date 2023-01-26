using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;
using PlatformService.AsyncDataServices;

namespace PlatformService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly IPlatformRepo repo;
    private readonly IMapper mapper;
    private readonly ICommandDataClient commandDataClient;
    private readonly IMessageBusClient messageBusClient;

    public PlatformsController(
      IPlatformRepo repo,
      IMapper mapper,
      ICommandDataClient commandDataClient,
      IMessageBusClient messageBusClient)
    {
        this.repo = repo;
        this.mapper = mapper;
        this.commandDataClient = commandDataClient;
        this.messageBusClient = messageBusClient;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
        System.Console.WriteLine("--> Getting Platforms");

        var platforms = repo.GetPlatforms();

        return Ok(mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
    }

    [HttpGet("{id}", Name = "GetPlatform")]
    public ActionResult<PlatformReadDto> GetPlatform(int id)
    {
        System.Console.WriteLine($"--> Getting Platform {id}");

        var platform = repo.GetPlatformById(id);

        return platform is not null ?
          Ok(mapper.Map<PlatformReadDto>(platform)) :
          NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<PlatformReadDto>> CreatePlatformAsync(PlatformCreateDto platformCreateDto)
    {
        var platform = mapper.Map<Platform>(platformCreateDto);
        repo.CreatePlatform(platform);
        repo.SaveChanges();

        var platformReadDto = mapper.Map<PlatformReadDto>(platform);

        // Send Sync Message
        try
        {
            await commandDataClient.SendPlatformToCommand(platformReadDto);
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
        }

        // Send Async Message
        try
        {
            var platformPublishedDto = mapper.Map<PlatformPublishedDto>(platformReadDto);
            platformPublishedDto.Event = "Platform_Published";
            messageBusClient.PublishNewPlatform(platformPublishedDto);
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
        }

        return CreatedAtRoute(nameof(GetPlatform), new { Id = platformReadDto.Id }, platformReadDto);
    }
}
