using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly IPlatformRepo repo;
    private readonly IMapper mapper;
    private readonly ICommandDataClient commandDataClient;

    public PlatformsController(
      IPlatformRepo repo,
      IMapper mapper,
      ICommandDataClient commandDataClient)
    {
        this.repo = repo;
        this.mapper = mapper;
        this.commandDataClient = commandDataClient;
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

        try
        {
            await commandDataClient.SendPlatformToCommand(platformReadDto);
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
        }

        return CreatedAtRoute(nameof(GetPlatform), new { Id = platformReadDto.Id }, platformReadDto);
    }
}
