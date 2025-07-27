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

        //A Curriculum can have multiple ExamBoards while an ExamBoard can only belong to one Curriculum.
        //Hence, there is a one-to-many relationship between Curriculum and ExamBoard
        modelBuilder
            .Entity<ExamBoard>()
            .HasOne(eb => eb.Curriculum)
            .WithMany()
            .HasForeignKey(eb => eb.CurriculumId)
            .OnDelete(DeleteBehavior.Cascade); //Delete Curriculum -> delete ExamBoards for that Curriculum

        //A Curriculum can have multiple Subjects and a Subject can exist in multiple Curriculums.
        //Hence, there is a many-to-many relationship between Curriculum and Subject
        modelBuilder.Entity<Curriculum>().HasMany(c => c.Subjects).WithMany(s => s.Curriculums);

        //A Question can have multiple Tags and a Tag can exist in multiple Questions.
        //Hence, there is a many-to-many relationship between Question and Tag
        modelBuilder.Entity<Question>().HasMany(q => q.Tags).WithMany(t => t.Questions);

        //A User can have multiple Likes while a Like can only belong to one User.
        //Hence, there is a one-to-many relationship between User and Like
        modelBuilder
            .Entity<Like>()
            .HasOne(l => l.User)
            .WithMany()
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.NoAction); //Delete User -> set the foreign UserId key to null

        //A Question can have multiple Likes while a Like can only belong to one Question.
        //Hence, there is a one-to-many relationship between Question and Like
        modelBuilder
            .Entity<Question>()
            .HasMany(q => q.Likes)
            .WithOne(l => l.Question)
            .HasForeignKey(l => l.QuestionId)
            .OnDelete(DeleteBehavior.Cascade); //Delete Question -> delete Likes for that question

        //A User can have multiple Upvotes while an Upvote can only belong to one User.
        //Hence, there is a one-to-many relationship between User and Upvote
        modelBuilder
            .Entity<Upvote>()
            .HasOne(upv => upv.User)
            .WithMany()
            .HasForeignKey(upv => upv.UserId)
            .OnDelete(DeleteBehavior.NoAction); //Delete User -> set the foreign UserId key to null

        //A Question can have multiple Upvotes while an Upvote can only belong to one Question.
        //Hence, there is a one-to-many relationship between Question and Upvote
        modelBuilder
            .Entity<Question>()
            .HasMany(q => q.Upvotes)
            .WithOne(upv => upv.Question)
            .HasForeignKey(upv => upv.QuestionId)
            .OnDelete(DeleteBehavior.Cascade); //Delete Question -> delete Upvotes for that question
    }
}
