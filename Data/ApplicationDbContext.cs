using Education.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; } = default!;
    public DbSet<UserOtp> UserOtps { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //A User can have multiple Otps while an Otp can only belong to one User.
        //Hence, there is a one-to-many relationship between User and UserOtp
        modelBuilder
            .Entity<UserOtp>()
            .HasOne(uo => uo.User)
            .WithMany()
            .HasForeignKey(uo => uo.UserId)
            .OnDelete(DeleteBehavior.Cascade); //Delete User -> delete UserOtps for that user
    }
}
