using Education.Api.Data;
using Education.Api.Dtos.Curriculums;
using Education.Api.Dtos.ExamBoards;
using Education.Api.Dtos.Levels;
using Education.Api.Dtos.Questions;
using Education.Api.Dtos.Subjects;
using Education.Api.Dtos.Tags;
using Education.Api.Dtos.Topics;
using Education.Api.Dtos.Topics.Subtopics;
using Education.Api.Dtos.Upvotes;
using Education.Api.Dtos.Users;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Questions;
using Education.Api.Services.Abstractions.Tags;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Services.Implementations.Questions;

public class QuestionService(
    ApplicationDbContext context,
    ILogger<QuestionService> logger,
    ITagService tagService
) : IQuestionService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<QuestionService> _logger = logger;
    private readonly ITagService _tagService = tagService;

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
                            LevelId = q.LevelId,
                            Level =
                                q.Level != null
                                    ? new LevelDto
                                    {
                                        Id = q.Level.Id,
                                        Name = q.Level.Name,
                                        ExamBoardId = q.Level.ExamBoardId,
                                        ExamBoard =
                                            q.Level.ExamBoard != null
                                                ? new ExamBoardDto
                                                {
                                                    Id = q.Level.ExamBoard.Id,
                                                    Name = q.Level.ExamBoard.Name,
                                                    CurriculumId = q.Level.ExamBoard.CurriculumId,
                                                    Curriculum =
                                                        q.Level.ExamBoard.Curriculum != null
                                                            ? new CurriculumDto
                                                            {
                                                                Id = q.Level
                                                                    .ExamBoard
                                                                    .Curriculum
                                                                    .Id,
                                                                Name = q.Level
                                                                    .ExamBoard
                                                                    .Curriculum
                                                                    .Name
                                                            }
                                                            : null,
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
                            Upvotes = q.Upvotes
                                .Select(
                                    upv =>
                                        new UpvoteDto
                                        {
                                            Id = upv.Id,
                                            UserId = upv.UserId,
                                            QuestionId = upv.QuestionId,
                                        }
                                )
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
                        LevelId = q.LevelId,
                        Level =
                            q.Level != null
                                ? new LevelDto
                                {
                                    Id = q.Level.Id,
                                    Name = q.Level.Name,
                                    ExamBoardId = q.Level.ExamBoardId,
                                    ExamBoard =
                                        q.Level.ExamBoard != null
                                            ? new ExamBoardDto
                                            {
                                                Id = q.Level.ExamBoard.Id,
                                                Name = q.Level.ExamBoard.Name,
                                                CurriculumId = q.Level.ExamBoard.CurriculumId,
                                                Curriculum =
                                                    q.Level.ExamBoard.Curriculum != null
                                                        ? new CurriculumDto
                                                        {
                                                            Id = q.Level.ExamBoard.Curriculum.Id,
                                                            Name = q.Level.ExamBoard.Curriculum.Name
                                                        }
                                                        : null,
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
                        Upvotes = q.Upvotes
                            .Select(
                                upv =>
                                    new UpvoteDto
                                    {
                                        Id = upv.Id,
                                        UserId = upv.UserId,
                                        QuestionId = upv.QuestionId,
                                    }
                            )
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
            TotalItems = totalItems,
            Items = questions
        };
    }

    /// <summary>
    /// Adds a new question to the database after validating the provided topic, subtopics, and tags.
    /// </summary>
    /// <param name="userId">The ID of the user creating the question.</param>
    /// <param name="dto">The question DTO containing question content, metadata, and tags.</param>
    /// <returns>The newly created <see cref="QuestionDto"/>.</returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when a referenced topic does not exist.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when one or more selected subtopics do not belong to the specified topic.
    /// </exception>
    public async Task<QuestionDto> AddAsync(int userId, AddQuestionDto dto)
    {
        //STEP 1: Check if a topic with the given ID exists
        var topic = await _context
            .Topics
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id.Equals(dto.TopicId));

        if (topic is null)
        {
            _logger.LogWarning(
                "Failed to add question: topic with ID {TopicId} not found.",
                dto.TopicId
            );

            throw new KeyNotFoundException(
                $"Unable to add question. Topic with ID '{dto.TopicId}' does not exist."
            );
        }

        //STEP 2: Get the selected subtopics and make sure they belong to the specified topic
        var selectedSubtopics = await _context
            .Subtopics
            .Where(st => dto.SubtopicIds.Contains(st.Id) && st.TopicId == dto.TopicId)
            .ToListAsync();
        if (selectedSubtopics.Count != dto.SubtopicIds.Count)
        {
            _logger.LogWarning(
                "Failed to add question: one or more selected subtopics do not exist under a topic with ID {TopicId}",
                dto.TopicId
            );

            throw new InvalidOperationException(
                $"Unable to add question: one or more selected subtopics do not exist under a topic with ID {dto.TopicId}"
            );
        }

        //STEP 3: Initialize the question entity with provided details
        Question question =
            new()
            {
                Content = dto.Content,
                SubjectId = topic.SubjectId,
                TopicId = dto.TopicId,
                UserId = userId,
                Marks = dto.Marks,
            };
        question.Subtopics.AddRange(selectedSubtopics);

        //STEP 4: Find each tag by name, or create it if it doesn't exist, then add it to the question.
        // Go through each tag name provided in the request.
        // Remove duplicates, ignoring case differences (e.g., "math" and "Math" are treated as the same).
        foreach (string tagName in dto.Tags.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            Tag tag = await _tagService.GetByNameAsync(tagName);
            question.Tags.Add(tag);
        }
        
        
        //STEP 5: If an answer was was provided create

        //STEP 7: Finally save the question to the database
        await _context.Questions.AddAsync(question);

        _logger.LogInformation(
            "Successfully created new question by user with ID {UserId}.",
            userId
        );

        return QuestionDto.MapFrom(question);
    }

    /// <summary>
    /// Updates an existing question with new content, topic, subtopics, tags, and related details.
    /// </summary>
    /// <param name="userId">The ID of the user attempting to update the question.</param>
    /// <param name="questionId">The ID of the question to update.</param>
    /// <param name="dto">The updated question data.</param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the question, exam board, subject, or topic is not found.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown if the question does not belong to the specified user.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if one or more subtopics are invalid for the selected topic.
    /// </exception>
    public async Task UpdateAsync(int userId, int questionId, UpdateQuestionDto dto)
    {
        //STEP 1: Check if a question with a give ID exists
        var existingQuestion = await _context
            .Questions
            .FirstOrDefaultAsync(q => q.Id.Equals(questionId));
        if (existingQuestion is null)
        {
            _logger.LogWarning("Question not found: {QuestionId}", questionId);

            throw new KeyNotFoundException($"Question with ID '{questionId}' does not exist.");
        }

        //STEP 2: Verify that the question belongs to the given user. Prevent updates by other users.
        if (existingQuestion.UserId != userId)
        {
            _logger.LogWarning(
                "Question with ID {QuestionId} doesn't belong to user with ID {UserId}",
                questionId,
                userId
            );

            throw new UnauthorizedAccessException(
                $"You're not authorized to update this question."
            );
        }

        //STEP 3: Check if an exam board with the given ID exists
        var examBoard = await _context
            .ExamBoards
            .AsNoTracking()
            .FirstOrDefaultAsync(eb => eb.Id.Equals(dto.ExamBoardId));

        if (examBoard is null)
        {
            _logger.LogWarning("Exam board with ID {ExamBoardId} not found.", dto.ExamBoardId);

            throw new KeyNotFoundException(
                $"Exam board with ID '{dto.ExamBoardId}' does not exist."
            );
        }

        //STEP 4: Check if a subject with the given ID exists and also belongs to the given Exam board
        var subject = await _context
            .Subjects
            .AsNoTracking()
            .FirstOrDefaultAsync(
                s => s.Id.Equals(dto.SubjectId) && s.ExamBoards.Contains(examBoard)
            );
        if (subject is null)
        {
            _logger.LogWarning(
                "No subject with ID {SubjectId} found for exam board with ID {ExamBoardId}.",
                dto.SubjectId,
                dto.ExamBoardId
            );

            throw new KeyNotFoundException(
                $"No subject found with ID '{dto.SubjectId}' for exam board with ID '{dto.ExamBoardId}'."
            );
        }
        //STEP 5: Check if a topic with the given ID exists and also belongs to the given subject
        var topic = await _context
            .Topics
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id.Equals(dto.TopicId) && t.Subjects.Contains(subject));

        if (topic is null)
        {
            _logger.LogWarning(
                "Topic with ID {TopicId} was not found for subject with ID {SubjectId}.",
                dto.TopicId,
                dto.SubjectId
            );

            throw new KeyNotFoundException(
                $"No topic with ID {dto.TopicId} found for subject with ID {dto.SubjectId}."
            );
        }
        //STEP 6: Get the selected subtopics and make sure they belong to the topic
        var selectedSubtopics = await _context
            .Subtopics
            .Where(st => dto.SubtopicIds.Contains(st.Id) && st.TopicId == dto.TopicId)
            .ToListAsync();
        if (selectedSubtopics.Count != dto.SubtopicIds.Count)
        {
            _logger.LogWarning(
                "One or more selected subtopics do not exist under a topic with ID {TopicId}",
                dto.TopicId
            );

            throw new InvalidOperationException(
                $"One or more selected subtopics do not exist under a topic with ID {dto.TopicId}"
            );
        }

        //STEP 7: Update the existing question entity with provided details
        existingQuestion.Content = dto.Content;
        existingQuestion.ExamBoardId = dto.ExamBoardId;
        existingQuestion.SubjectId = dto.SubjectId;
        existingQuestion.TopicId = dto.TopicId;
        existingQuestion.Subtopics.AddRange(selectedSubtopics);
        existingQuestion.Marks = dto.Marks;

        //STEP 8: Find each tag by name, or create it if it doesn't exist, then add it to the question.
        // Go through each tag name provided in the request.
        // Remove duplicates, ignoring case differences (e.g., "math" and "Math" are treated as the same).
        foreach (string tagName in dto.Tags.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            Tag tag = await _tagService.GetByNameAsync(tagName);
            existingQuestion.Tags.Add(tag);
        }

        //STEP 9: Finally persist the changes to the database
        await _context.SaveChangesAsync();

        _logger.LogInformation("Successfully updated question with ID {QuestionId}.", questionId);
    }

    /// <summary>
    ///  Deletes an existing question with a given ID.
    /// </summary>
    /// <param name="userId">The ID of the user attempting to delete the question.</param>
    /// <param name="questionId">The ID of the soon to be deleted question.</param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the question is not found.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown if the question does not belong to the specified user.
    /// </exception>
    /// <remarks>
    /// Deleting a question does not automatically delete its related comments and upvotes because
    /// the cascade delete behavior is set to NoAction.
    /// The comments and upvotes must be deleted manually to avoid orphaned records.
    /// </remarks>
    public async Task DeleteAsync(int userId, int questionId)
    {
        //Check if the question exists
        var question = await _context.Questions.FirstOrDefaultAsync(q => q.Id == questionId);

        if (question is null)
        {
            _logger.LogWarning("Delete failed. Question not found: {QuestionId}", questionId);

            throw new KeyNotFoundException(
                $"Delete failed. Question with ID '{questionId}' does not exist."
            );
        }
        //Make sure the question belongs to the specified user
        if (question.UserId != userId)
        {
            _logger.LogWarning(
                "Delete failed. Question {QuestionId} does not belong to user {UserId}.",
                questionId,
                userId
            );

            throw new UnauthorizedAccessException("You're not authorized delete this question.");
        }

        //delete the question
        _context.Questions.Remove(question);

        // Manually delete all comments linked to this question
        await _context.Comments.Where(c => c.QuestionId == questionId).ExecuteDeleteAsync();

        // Manually delete all upvotes linked to this question
        await _context.Upvotes.Where(up => up.QuestionId == questionId).ExecuteDeleteAsync();

        _logger.LogInformation("Successfully deleted question: {QuestionId}", questionId);
    }
}
