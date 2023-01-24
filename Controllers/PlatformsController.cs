using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
  private readonly IPlatformRepo repo;
  private readonly IMapper mapper;

  public PlatformsController(IPlatformRepo repo, IMapper mapper)
  {
    this.repo = repo;
    this.mapper = mapper;
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
  public ActionResult<PlatformReadDto> CreatePlatform(PlatformCreateDto platformCreateDto)
  {
    var platform = mapper.Map<Platform>(platformCreateDto);
    repo.CreatePlatform(platform);
    repo.SaveChanges();

    var platformReadDto = mapper.Map<PlatformReadDto>(platform);

    return CreatedAtRoute(nameof(GetPlatform), new { Id = platformReadDto.Id }, platformReadDto);
  }
}
