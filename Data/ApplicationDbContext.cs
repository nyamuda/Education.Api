using Education.Api.Models;
using Education.Api.Models.Topics;
using Education.Api.Models.Users;
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

        //CURRICULUM

        //A Curriculum can have multiple Subjects and a Subject can exist in multiple Curriculums.
        //Hence, there is a many-to-many relationship between Curriculum and Subject.
        modelBuilder.Entity<Curriculum>().HasMany(c => c.Subjects).WithMany(s => s.Curriculums);

        //A Question can only belong to one Curriculum and a Curriculum can have multiple Questions.
        //Hence, there is a one-to-many relationship between Curriculum and Question.
        modelBuilder
            .Entity<Question>()
            .HasOne(q => q.Curriculum)
            .WithMany(c => c.Questions)
            .HasForeignKey(q => q.CurriculumId)
            .OnDelete(DeleteBehavior.Cascade);

        //A Curriculum can have multiple ExamBoards while an ExamBoard can only belong to one Curriculum.
        //Hence, there is a one-to-many relationship between Curriculum and ExamBoard
        modelBuilder
            .Entity<ExamBoard>()
            .HasOne(eb => eb.Curriculum)
            .WithMany()
            .HasForeignKey(eb => eb.CurriculumId)
            .OnDelete(DeleteBehavior.Cascade); //Delete Curriculum -> delete ExamBoards for that Curriculum

        //An ExamBoard can have multiple Subjects and a Subject can exist in multiple ExamBoards.
        //Hence, there is a many-to-many relationship between ExamBoard and Subject.
        modelBuilder.Entity<ExamBoard>().HasMany(eb => eb.Subjects).WithMany(s => s.ExamBoards);

        //An ExamBoard can have multiple Questions and a Question can only belong to one ExamBoard.
        //Hence, there is a one-to-many relationship between ExamBoard and Question.
        modelBuilder
            .Entity<ExamBoard>()
            .HasMany(eb => eb.Questions)
            .WithOne(q => q.ExamBoard)
            .HasForeignKey(q => q.ExamBoardId)
            .OnDelete(DeleteBehavior.Cascade);

        //A Question can have multiple Tags and a Tag can exist in multiple Questions.
        //Hence, there is a many-to-many relationship between Question and Tag
        modelBuilder.Entity<Question>().HasMany(q => q.Tags).WithMany(t => t.Questions);

        //A Subject can have multiple Questions while a Question can only belong to one Subject.
        //Hence, there is a one-to-many relationship between Subject and Question.
        modelBuilder
            .Entity<Question>()
            .HasOne(q => q.Subject)
            .WithMany(s => s.Questions)
            .HasForeignKey(q => q.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);
        //A Topic can have multiple Questions while a Question can only belong to one Topic.
        //Hence, there is a one-to-many relationship between Topic and Question.
        modelBuilder
            .Entity<Question>()
            .HasOne(q => q.Topic)
            .WithMany(t => t.Questions)
            .HasForeignKey(q => q.TopicId)
            .OnDelete(DeleteBehavior.Cascade);

        //A Topic can have multiple Subtopics while a Subtopic can only belong to one Topic.
        //Hence, there is a one-to-many relationship between Topic and Subtopic.
        modelBuilder
            .Entity<Subtopic>()
            .HasOne(st => st.Topic)
            .WithMany(t => t.Subtopics)
            .HasForeignKey(st => st.TopicId)
            .OnDelete(DeleteBehavior.Cascade);

        //A User can have multiple Likes while a Like can only belong to one User.
        //Hence, there is a one-to-many relationship between User and Like.
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

        //A Question can have multiple Answers while an Answer can only belong to one Question.
        //Hence, there is a one-to-many relationship between Question and Answer
        modelBuilder
            .Entity<Question>()
            .HasMany(q => q.Answers)
            .WithOne(a => a.Question)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        //An Answer can have multiple Upvotes while an Upvote can only belong to one Answer.
        //Hence, there is a one-to-many relationship between Answer and Upvote
        modelBuilder
            .Entity<Answer>()
            .HasMany(a => a.Upvotes)
            .WithOne(upv => upv.Answer)
            .HasForeignKey(upv => upv.AnswerId)
            .OnDelete(DeleteBehavior.Cascade); //Delete Answer -> delete Upvotes for that answer

        //An Answer can have multiple Likes while a Like can only belong to one Answer.
        //Hence, there is a one-to-many relationship between Answer and Like
        modelBuilder
            .Entity<Answer>()
            .HasMany(a => a.Likes)
            .WithOne(l => l.Answer)
            .HasForeignKey(l => l.AnswerId)
            .OnDelete(DeleteBehavior.Cascade); //Delete Answer -> delete Likes for that answer

        //An User can have multiple Question while a Question can only belong to one User.
        //Hence, there is a one-to-many relationship between User and Question
        modelBuilder
            .Entity<Question>()
            .HasOne(q => q.User)
            .WithMany()
            .HasForeignKey(q => q.UserId)
            .OnDelete(DeleteBehavior.NoAction); //Delete User -> set foreign to null
    }
}
