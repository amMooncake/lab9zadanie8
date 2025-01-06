using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Models;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public DbSet<ContactEntity> Contacts { get; set; }
    
    private string DbPath { get; set; }
    public AppDbContext()
    {

    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    var ADMIN_ID = Guid.NewGuid().ToString();
    var ADMIN_ROLE_ID = Guid.NewGuid().ToString();
    var USER_ID = Guid.NewGuid().ToString();
    var USER_ROLE_ID = Guid.NewGuid().ToString();

    modelBuilder.Entity<IdentityRole>()
        .HasData(new IdentityRole()
            {
                Id = ADMIN_ROLE_ID,
                Name = "admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = ADMIN_ROLE_ID
            },
            new IdentityRole()
            {
                Id = USER_ROLE_ID,
                Name = "user",
                NormalizedName = "USER",
                ConcurrencyStamp = USER_ROLE_ID
            }
        );

    var admin = new IdentityUser()
    {
        Id = ADMIN_ID,
        UserName = "Aleksy",
        NormalizedUserName = "ALEKSY",
        Email = "aleksy@gmail.com",
        NormalizedEmail = "ALEKSY@GMAIL.COM",
        EmailConfirmed = true
    };

    var user = new IdentityUser()
    {
        Id = USER_ID,
        UserName = "Maks",
        NormalizedUserName = "MAKS",
        Email = "maks@gmail.com",
        NormalizedEmail = "MAKS@GMAIL.COM",
        EmailConfirmed = true
    };

    var hasher = new PasswordHasher<IdentityUser>();
    admin.PasswordHash = hasher.HashPassword(admin, "1234!");
    user.PasswordHash = hasher.HashPassword(user, "1234");

    modelBuilder.Entity<IdentityUser>()
        .HasData(admin, user);

    modelBuilder.Entity<IdentityUserRole<string>>()
        .HasData(
            new IdentityUserRole<string>
            {
                RoleId = ADMIN_ROLE_ID,
                UserId = ADMIN_ID
            },
            new IdentityUserRole<string>
            {
                RoleId = USER_ROLE_ID,
                UserId = USER_ID
            },
            new IdentityUserRole<string>
            {
                RoleId = USER_ROLE_ID,
                UserId = ADMIN_ID
            }
        );
}
}