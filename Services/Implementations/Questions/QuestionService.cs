using Education.Api.Data;
using Education.Api.Dtos.Curriculums;
using Education.Api.Dtos.ExamBoards;
using Education.Api.Dtos.Questions;
using Education.Api.Dtos.Subjects;
using Education.Api.Dtos.Tags;
using Education.Api.Dtos.Topics;
using Education.Api.Dtos.Topics.Subtopics;
using Education.Api.Dtos.Users;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Questions;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Services.Implementations.Questions;

public class QuestionService(ApplicationDbContext context) : IQuestionService
{
    private readonly ApplicationDbContext _context = context;

    //Gets a question with a given ID
    public async Task<QuestionDto> GetByIdAsync(int id)
    {
        return await _context
                .Questions
                .AsNoTracking()
                .Select(
                    q =>
                        new QuestionDto
                        {
                            Id = q.Id,
                            Content = q.Content,
                            Marks = q.Marks,
                            ExamBoardId = q.ExamBoardId,
                            ExamBoard =
                                q.ExamBoard != null
                                    ? new ExamBoardDto
                                    {
                                        Id = q.ExamBoard.Id,
                                        Name = q.ExamBoard.Name,
                                        CurriculumId = q.ExamBoard.CurriculumId,
                                        Curriculum =
                                            q.ExamBoard.Curriculum != null
                                                ? new CurriculumDto
                                                {
                                                    Id = q.ExamBoard.Curriculum.Id,
                                                    Name = q.ExamBoard.Curriculum.Name
                                                }
                                                : null
                                    }
                                    : null,
                            SubjectId = q.SubjectId,
                            Subject =
                                q.Subject != null
                                    ? new SubjectDto { Id = q.Subject.Id, Name = q.Subject.Name, }
                                    : null,
                            TopicId = q.TopicId,
                            Topic =
                                q.Topic != null
                                    ? new TopicDto { Id = q.Topic.Id, Name = q.Topic.Name }
                                    : null,
                            Subtopics = q.Subtopics
                                .Select(
                                    st =>
                                        new SubtopicDto
                                        {
                                            Id = st.Id,
                                            Name = st.Name,
                                            TopicId = st.TopicId
                                        }
                                )
                                .ToList(),
                            UserId = q.UserId,
                            User =
                                q.User != null
                                    ? new UserDto { Id = q.User.Id, Username = q.User.Username }
                                    : null,
                            Tags = q.Tags
                                .Select(t => new TagDto { Id = t.Id, Name = t.Name, })
                                .ToList(),
                            CreatedAt = q.CreatedAt,
                            UpdatedAt = q.UpdatedAt
                        }
                )
                .FirstOrDefaultAsync(q => q.Id.Equals(id))
            ?? throw new KeyNotFoundException($"Question with ID '{id}' does not exist.");
    }

    /// <summary>
    /// Retrieves a paginated list of questions.
    /// </summary>
    /// <param name="page">The current page number.</param>
    /// <param name="pageSize">The number of items to include per page.</param>
    /// <returns>
    /// A <see cref="PageInfo{QuestionDto}"/> containing the list of question for the specified page,
    /// along with pagination metadata such as page number, page size, and whether more items are available.
    /// </returns>
    public async Task<PageInfo<QuestionDto>> GetAsync(int page, int pageSize)
    {
        var query = _context.Questions.AsQueryable();

        List<QuestionDto> questions = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(
                q =>
                    new QuestionDto
                    {
                        Id = q.Id,
                        Content = q.Content,
                        Marks = q.Marks,
                        ExamBoardId = q.ExamBoardId,
                        ExamBoard =
                            q.ExamBoard != null
                                ? new ExamBoardDto
                                {
                                    Id = q.ExamBoard.Id,
                                    Name = q.ExamBoard.Name,
                                    CurriculumId = q.ExamBoard.CurriculumId,
                                    Curriculum =
                                        q.ExamBoard.Curriculum != null
                                            ? new CurriculumDto
                                            {
                                                Id = q.ExamBoard.Curriculum.Id,
                                                Name = q.ExamBoard.Curriculum.Name
                                            }
                                            : null
                                }
                                : null,
                        SubjectId = q.SubjectId,
                        Subject =
                            q.Subject != null
                                ? new SubjectDto { Id = q.Subject.Id, Name = q.Subject.Name, }
                                : null,
                        TopicId = q.TopicId,
                        Topic =
                            q.Topic != null
                                ? new TopicDto { Id = q.Topic.Id, Name = q.Topic.Name }
                                : null,
                        Subtopics = q.Subtopics
                            .Select(
                                st =>
                                    new SubtopicDto
                                    {
                                        Id = st.Id,
                                        Name = st.Name,
                                        TopicId = st.TopicId
                                    }
                            )
                            .ToList(),
                        UserId = q.UserId,
                        User =
                            q.User != null
                                ? new UserDto { Id = q.User.Id, Username = q.User.Username }
                                : null,
                        Tags = q.Tags
                            .Select(t => new TagDto { Id = t.Id, Name = t.Name, })
                            .ToList(),
                        CreatedAt = q.CreatedAt,
                        UpdatedAt = q.UpdatedAt
                    }
            )
            .ToListAsync();

        int totalItems = await query.CountAsync();
        bool hasMore = totalItems > page * pageSize;

        return new PageInfo<QuestionDto>
        {
            Page = page,
            PageSize = pageSize,
            HasMore = hasMore,
            Items = questions
        };
    }

    Task<QuestionDto> AddAsync(AddQuestionDto dto);

    Task UpdateAsync(int id, UpdateQuestionDto dto);

    Task DeleteAsync(int id);
}
