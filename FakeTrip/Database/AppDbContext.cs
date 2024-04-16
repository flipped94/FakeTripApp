using FakeTrip.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Reflection;

namespace FakeTrip.Databases;

public class AppDbContext : IdentityDbContext<IdentityUser>  /* DbContext*/
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TouristRoute> TouristRoutes { get; set; }

    public DbSet<TouristRoutePicture> TouristRoutePictures { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var touristRouteJsonData = File.ReadAllText(Path.GetDirectoryName(
            Assembly.GetExecutingAssembly().Location) + @"/Database/touristRoutesMockData.json");
        IList<TouristRoute> touristRoutes = JsonConvert.DeserializeObject<IList<TouristRoute>>(touristRouteJsonData)!;
        modelBuilder.Entity<TouristRoute>().HasData(touristRoutes);

        var touristRoutePictureJsonData = File.ReadAllText(Path.GetDirectoryName(
            Assembly.GetExecutingAssembly().Location) + @"/Database/touristRoutePicturesMockData.json");
        IList<TouristRoutePicture> touristRoutePictures = JsonConvert.DeserializeObject<IList<TouristRoutePicture>>(touristRoutePictureJsonData)!;
        modelBuilder.Entity<TouristRoutePicture>().HasData(touristRoutePictures);

        base.OnModelCreating(modelBuilder);
    }
}
