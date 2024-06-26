﻿using FakeTrip.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Reflection;

namespace FakeTrip.Databases;

public class AppDbContext : IdentityDbContext<ApplicationUser>  /* DbContext*/
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TouristRoute> TouristRoutes { get; set; }

    public DbSet<TouristRoutePicture> TouristRoutePictures { get; set; }

    public DbSet<ShoppingCart> ShoppingCarts { get; set; }

    public DbSet<LineItem> LineItems { get; set; }

    public DbSet<Order> Orders { get; set; }

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

        modelBuilder.Entity<ApplicationUser>(b => {
            b.HasMany(x => x.UserRoles)
            .WithOne()
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();
        });

        // 添加角色
        var adminRoleId = "308660dc-ae51-480f-824d-7dca6714c3e2"; // guid 
        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = adminRoleId,
                Name = "Admin",
                NormalizedName = "Admin".ToUpper()
            }
        );

        // 添加用户
        var adminUserId = "90184155-dee0-40c9-bb1e-b5ed07afc04e";
        ApplicationUser adminUser = new ApplicationUser
        {
            Id = adminUserId,
            UserName = "admin@faketrip.com",
            NormalizedUserName = "admin@faketrip.com".ToUpper(),
            Email = "admin@faketrip.com",
            NormalizedEmail = "admin@faketrip.com".ToUpper(),
            TwoFactorEnabled = false,
            EmailConfirmed = true,
            PhoneNumber = "123456789",
            PhoneNumberConfirmed = false
        };
        PasswordHasher<ApplicationUser> ph = new PasswordHasher<ApplicationUser>();
        adminUser.PasswordHash = ph.HashPassword(adminUser, "Fake123$");
        modelBuilder.Entity<ApplicationUser>().HasData(adminUser);

        // 给用户加入管理员权限
        // 通过使用 linking table：IdentityUserRole
        modelBuilder.Entity<IdentityUserRole<string>>()
            .HasData(new IdentityUserRole<string>()
            {
                RoleId = adminRoleId,
                UserId = adminUserId
            });

        base.OnModelCreating(modelBuilder);
    }
}
