using PlatformService.Models;

namespace PlatformService.Data;

public class PlatformRepo : IPlatformRepo
{
    private readonly AppDbContext context;

    public PlatformRepo(AppDbContext context)
    {
        this.context = context;
    }

    public void CreatePlatform(Platform platform)
    {
        if (platform is null) throw new ArgumentNullException(nameof(platform));

        context.Platforms.Add(platform);
    }

    public Platform GetPlatformById(int id)
    {
        return context.Platforms.FirstOrDefault(p => p.Id == id);
    }

    public IEnumerable<Platform> GetPlatforms()
    {
        return context.Platforms.ToList();
    }

    public bool SaveChanges()
    {
        return (context.SaveChanges() >= 0);
    }
}
