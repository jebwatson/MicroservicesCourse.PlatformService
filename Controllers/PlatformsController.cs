using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;

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
}
