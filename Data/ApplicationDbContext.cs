using Education.Api.Enums;
using Education.Api.Models;
using Education.Api.Models.Flags;
using Education.Api.Models.Topics;
using Education.Api.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<Answer> Answers { get; set; }
    public DbSet<AnswerFlag> AnswerFlags { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<CommentFlag> CommentFlags { get; set; }
    public DbSet<Curriculum> Curriculums { get; set; }
    public DbSet<ExamBoard> ExamBoards { get; set; }
    public DbSet<Level> Levels { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<QuestionBookmark> QuestionBookmarks { get; set; }
    public DbSet<QuestionFlag> QuestionFlags { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Subtopic> Subtopics { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Topic> Topics { get; set; }
    public DbSet<Upvote> Upvotes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserOtp> UserOtps { get; set; }

    // Configure the DbContext and seed a specific user as an Admin during initialization
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        string email = "ptnrlab@gmail.com";

        //make a user an admin
        optionsBuilder
            .UseSeeding(
                (context, _) =>
                {
                    // Find user by email
                    var user = context.Set<User>().FirstOrDefault(u => u.Email.Equals(email));
                    // If found, assign Admin role if they haven't been assigned that role
                    if (user is not null && user.Role != UserRole.Admin)
                    {
                        user.Role = UserRole.Admin;
                        context.SaveChanges();
                    }
                }
            )
            .UseAsyncSeeding(
                async (context, _, cancellationToken) =>
                {
                    // Find user by email
                    var user = await context
                        .Set<User>()
                        .FirstOrDefaultAsync(
                            u => u.Email.Equals(email),
                            cancellationToken: cancellationToken
                        );
                    // If found, assign Admin role if they haven't been assigned that role
                    if (user is not null && user.Role != UserRole.Admin)
                    {
                        user.Role = UserRole.Admin;
                        await context.SaveChangesAsync(cancellationToken);
                    }
                }
            );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //A Comment can have multiple CommentFlags while a CommentFlag can only belong to one Comment.
        //Hence, there is a one-to-many relationship between Comment and CommentFlag
        modelBuilder
            .Entity<Comment>()
            .HasMany(c => c.Flags)
            .WithOne(cf => cf.Comment)
            .HasForeignKey(cf => cf.CommentId)
            .OnDelete(DeleteBehavior.Cascade); //Delete comment -> delete flags for that comment

        //A Comment can have multiple Upvotes while an Upvote can only belong to one Comment.
        //Hence, there is a one-to-many relationship between Comment and Upvote
        modelBuilder
            .Entity<Comment>()
            .HasMany(c => c.Upvotes)
            .WithOne(uv => uv.Comment)
            .HasForeignKey(uv => uv.CommentId)
            .OnDelete(DeleteBehavior.NoAction); //Delete Comment -> set foreign key to null

        //A Comment can only belong to one User while a User can have multiple Comments.
        //Hence, there is a many-to-one relationship between Comment and User
        modelBuilder
            .Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.NoAction); //Delete User -> set foreign key UserId to null

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
            .WithMany(c => c.ExamBoards)
            .HasForeignKey(eb => eb.CurriculumId)
            .OnDelete(DeleteBehavior.Cascade); //Delete Curriculum -> delete ExamBoards for that Curriculum

        //An ExamBoard can have multiple Levels and a Level can only belong to one ExamBoard.
        //Hence, there is a one-to-many relationship between ExamBoard and Level.
        modelBuilder
            .Entity<ExamBoard>()
            .HasMany(eb => eb.Levels)
            .WithOne(l => l.ExamBoard)
            .HasForeignKey(l => l.ExamBoardId)
            .OnDelete(DeleteBehavior.Cascade); //Delete ExamBoard -> delete Levels for that exam board

        // A Level can have multiple Subjects and a Subject can only belong to one Level.
        // This is so because each subject in each level has its own properties (like topics, code, description).
        // This means the “Physics” in Grade 10 is fundamentally not the same entity as
        // “Physics” in Grade 11 — it just happens to share a name.
        // Hence, there is a one-to-many relationship between Level and Subject.
        modelBuilder
            .Entity<Subject>()
            .HasOne(s => s.Level)
            .WithMany(l => l.Subjects)
            .HasForeignKey(s => s.LevelId)
            .OnDelete(DeleteBehavior.NoAction); //Delete Level -> set foreign key to null

        //A Subject can have multiple Topics and a Topic can only belong to one Subject.
        //Hence, there is a one-to-many relationship between Subject and Topic.
        modelBuilder
            .Entity<Subject>()
            .HasMany(s => s.Topics)
            .WithOne(t => t.Subject)
            .HasForeignKey(t => t.SubjectId)
            .OnDelete(DeleteBehavior.NoAction); //Delete Subject -> set foreign key to null

        //A Topic can have multiple Subtopics while a Subtopic can only belong to one Topic.
        //Hence, there is a one-to-many relationship between Topic and Subtopic.
        modelBuilder
            .Entity<Subtopic>()
            .HasOne(st => st.Topic)
            .WithMany(t => t.Subtopics)
            .HasForeignKey(st => st.TopicId)
            .OnDelete(DeleteBehavior.Cascade); //Delete Topic -> delete Subtopics for that Topic

        //A User can have multiple Upvotes while an Upvote can only belong to one User.
        //Hence, there is a one-to-many relationship between User and Upvote
        modelBuilder
            .Entity<Upvote>()
            .HasOne(upv => upv.User)
            .WithMany()
            .HasForeignKey(upv => upv.UserId)
            .OnDelete(DeleteBehavior.NoAction); //Delete User -> set the foreign UserId key to null

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
            .OnDelete(DeleteBehavior.Cascade); //Delete Subject -> delete Questions for that subject

        //A Topic can have multiple Questions while a Question can only belong to one Topic.
        //Hence, there is a one-to-many relationship between Topic and Question.
        modelBuilder
            .Entity<Question>()
            .HasOne(q => q.Topic)
            .WithMany(t => t.Questions)
            .HasForeignKey(q => q.TopicId)
            .OnDelete(DeleteBehavior.Cascade); //Delete Topic -> delete Questions for that topic

        //A Question can have multiple Subtopics and a Subtopic can exist in multiple Questions
        //Hence, there is a many-to-many relationship between Question and Subtopic
        // modelBuilder
        //     .Entity<Question>()
        //     .HasMany(q => q.Subtopics)
        //     .WithMany(s => s.Questions)
        //     .UsingEntity<QuestionSubtopic>(
        //         r => r.HasOne<Subtopic>().WithMany().OnDelete(DeleteBehavior.NoAction),
        //         l => l.HasOne<Question>().WithMany().OnDelete(DeleteBehavior.NoAction)
        //     );

        //A Question can only be under one Subtopic and a Subtopic have multiple Questions.
        //Hence, there is a many-to-one relationship between Question and Subtopic
        modelBuilder
            .Entity<Question>()
            .HasOne(q => q.Subtopic)
            .WithMany(s => s.Questions)
            .HasForeignKey(q => q.SubtopicId)
            .OnDelete(DeleteBehavior.NoAction); //Delete Subtopic -> set foreign key to null

        //A Question can have multiple Upvotes while an Upvote can only belong to one Question.
        //Hence, there is a one-to-many relationship between Question and Upvote
        modelBuilder
            .Entity<Question>()
            .HasMany(q => q.Upvotes)
            .WithOne(upv => upv.Question)
            .HasForeignKey(upv => upv.QuestionId)
            .OnDelete(DeleteBehavior.NoAction); //Delete Question -> set foreign key to null

        //A Question can have multiple Answers while an Answer can only belong to one Question.
        //Hence, there is a one-to-many relationship between Question and Answer
        modelBuilder
            .Entity<Question>()
            .HasMany(q => q.Answers)
            .WithOne(a => a.Question)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Cascade); //Delete Question -> delete Answers for that question

        //A Question can have multiple Comments while a Comment can only belong to one Question.
        //Hence, there is a one-to-many relationship between Question and Comment
        modelBuilder
            .Entity<Question>()
            .HasMany(q => q.Comments)
            .WithOne(c => c.Question)
            .HasForeignKey(c => c.QuestionId)
            .OnDelete(DeleteBehavior.NoAction); //Delete Question -> Set foreign key to null

        //An User can have multiple Question while a Question can only belong to one User.
        //Hence, there is a one-to-many relationship between User and Question
        modelBuilder
            .Entity<Question>()
            .HasOne(q => q.User)
            .WithMany()
            .HasForeignKey(q => q.UserId)
            .OnDelete(DeleteBehavior.NoAction); //Delete User -> set foreign key UserId to null

        //An Question can have multiple QuestionFlags while a QuestionFlag can only belong to one Question.
        //Hence, there is a one-to-many relationship between Question and QuestionFlag
        modelBuilder
            .Entity<Question>()
            .HasMany(q => q.Flags)
            .WithOne(qf => qf.Question)
            .HasForeignKey(qf => qf.QuestionId)
            .OnDelete(DeleteBehavior.Cascade); //Delete Question -> delete flags for that question

        //A Question can be bookmarked multiple times while a Bookmark can only only for one Question.
        //Hence, there is one-to-many relationship between Question and Bookmark.
        modelBuilder
            .Entity<Question>()
            .HasMany(q => q.Bookmarks)
            .WithOne(qb => qb.Question)
            .HasForeignKey(qb => qb.QuestionId)
            .OnDelete(DeleteBehavior.Cascade); //Delete Question -> delete Bookmarks for that question

        //An Answer can have multiple Upvotes while an Upvote can only belong to one Answer.
        //Hence, there is a one-to-many relationship between Answer and Upvote
        modelBuilder
            .Entity<Answer>()
            .HasMany(a => a.Upvotes)
            .WithOne(upv => upv.Answer)
            .HasForeignKey(upv => upv.AnswerId)
            .OnDelete(DeleteBehavior.Cascade); //Delete Answer -> delete Upvotes for that answer

        //An Answer can have multiple Comments while a Comment can only belong to one Answer.
        //Hence, there is a one-to-many relationship between Answer and Comment
        modelBuilder
            .Entity<Answer>()
            .HasMany(a => a.Comments)
            .WithOne(c => c.Answer)
            .HasForeignKey(c => c.AnswerId)
            .OnDelete(DeleteBehavior.Cascade); //Delete Answer -> delete comments for that answer

        //An User can have multiple Answers while an Answer can only belong to one User.
        //Hence, there is a one-to-many relationship between User and Answer
        modelBuilder
            .Entity<Answer>()
            .HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.NoAction); //Delete User -> set foreign key UserId to null

        //An Answer can have multiple AnswerFlags while an AnswerFlag can only belong to one Answer.
        //Hence, there is a one-to-many relationship between Answer and AnswerFlag
        modelBuilder
            .Entity<Answer>()
            .HasMany(a => a.Flags)
            .WithOne(af => af.Answer)
            .HasForeignKey(af => af.AnswerId)
            .OnDelete(DeleteBehavior.Cascade); //Delete Answer -> delete flags for that answer

        //An User can have multiple QuestionFlags while a QuestionFlag can only belong to one User.
        //Hence, there is a one-to-many relationship between User and QuestionFlag
        modelBuilder
            .Entity<QuestionFlag>()
            .HasOne(qf => qf.User)
            .WithMany()
            .HasForeignKey(qf => qf.UserId)
            .OnDelete(DeleteBehavior.NoAction); //Delete User -> set foreign key UserId to null

        //An User can have multiple AnswerFlags while an AnswerFlag can only belong to one User.
        //Hence, there is a one-to-many relationship between User and AnswerFlag
        modelBuilder
            .Entity<AnswerFlag>()
            .HasOne(af => af.User)
            .WithMany()
            .HasForeignKey(af => af.UserId)
            .OnDelete(DeleteBehavior.NoAction); //Delete User -> set foreign key UserId to null

        //An User can have multiple CommentFlags while a CommentFlag can only belong to one User.
        //Hence, there is a one-to-many relationship between User and CommentFlag
        modelBuilder
            .Entity<CommentFlag>()
            .HasOne(cf => cf.User)
            .WithMany()
            .HasForeignKey(cf => cf.UserId)
            .OnDelete(DeleteBehavior.NoAction); //Delete User -> set foreign key UserId to null

        //A User can only study one Curriculum while a Curriculum can have multiple Users.
        //Hence, there is a one-to-many relationship between Curriculum and User
        modelBuilder
            .Entity<User>()
            .HasOne(u => u.Curriculum)
            .WithMany()
            .HasForeignKey(u => u.CurriculumId)
            .OnDelete(DeleteBehavior.NoAction); //Delete Curriculum -> set foreign key UserId to null

        //A User can only study one ExamBoard while an ExamBoard can have multiple Users.
        //Hence, there is a one-to-many relationship between ExamBoard and User
        modelBuilder
            .Entity<User>()
            .HasOne(u => u.ExamBoard)
            .WithMany()
            .HasForeignKey(u => u.ExamBoardId)
            .OnDelete(DeleteBehavior.NoAction); //Delete ExamBoard -> set foreign key UserId to null

        // A User can Bookmark multiple questions while a QuestionBookmark
        // can only be for one User.
        // Hence, there is a one-to-many relationship between User and QuestionBookmark
        modelBuilder
            .Entity<User>()
            .HasMany(u => u.QuestionBookmarks)
            .WithOne(qb => qb.User)
            .HasForeignKey(qb => qb.UserId)
            .OnDelete(DeleteBehavior.Cascade); //Delete User -> delete question bookmarks for that User

        // A User can be enrolled in multiple educational Levels and an
        // educational Level can be taken by multiple Users.
        // Hence, there is a many-to-many relationship between User and Level
        modelBuilder.Entity<User>().HasMany(u => u.Levels).WithMany();
    }
}
