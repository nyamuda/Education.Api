using Education.Api.Data;
using Education.Api.Dtos.Answers;
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
using Education.Api.Services.Abstractions.Answers;
using Education.Api.Services.Abstractions.Questions;
using Education.Api.Services.Abstractions.Tags;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Services.Implementations.Questions;

public class QuestionService(
    ApplicationDbContext context,
    ILogger<QuestionService> logger,
    ITagService tagService,
    IAnswerService answerService
) : IQuestionService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<QuestionService> _logger = logger;
    private readonly ITagService _tagService = tagService;
    private readonly IAnswerService _answerService = answerService;

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
                            ContentText = q.ContentText,
                            ContentHtml = q.ContentHtml,
                            Marks = q.Marks,
                            SubjectId = q.SubjectId,
                            Subject =
                                q.Subject != null
                                    ? new SubjectDto
                                    {
                                        Id = q.Subject.Id,
                                        Name = q.Subject.Name,
                                        LevelId = q.Subject.LevelId
                                    }
                                    : null,
                            TopicId = q.TopicId,
                            Topic =
                                q.Topic != null
                                    ? new TopicDto
                                    {
                                        Id = q.Topic.Id,
                                        Name = q.Topic.Name,
                                        SubjectId = q.Topic.SubjectId
                                    }
                                    : null,
                            Subtopic =
                                q.Subtopic != null
                                    ? new SubtopicDto
                                    {
                                        Id = q.Subtopic.Id,
                                        Name = q.Subtopic.Name,
                                        TopicId = q.Subtopic.TopicId
                                    }
                                    : null,
                            UserId = q.UserId,
                            User =
                                q.User != null
                                    ? new UserDto { Id = q.User.Id, Username = q.User.Username }
                                    : null,
                            AuthorAnswer = q.Answers
                                .Where(a => a.UserId == q.UserId)
                                .Select(
                                    a =>
                                        new AnswerDto
                                        {
                                            Id = a.Id,
                                            UserId = a.UserId,
                                            ContentHtml = a.ContentHtml,
                                            ContentText = a.ContentText,
                                            QuestionId = a.QuestionId
                                        }
                                )
                                .FirstOrDefault(),
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
                        ContentText = q.ContentText,
                        ContentHtml = q.ContentHtml,
                        Marks = q.Marks,
                        SubjectId = q.SubjectId,
                        Subject =
                            q.Subject != null
                                ? new SubjectDto
                                {
                                    Id = q.Subject.Id,
                                    Name = q.Subject.Name,
                                    LevelId = q.Subject.LevelId
                                }
                                : null,
                        TopicId = q.TopicId,
                        Topic =
                            q.Topic != null
                                ? new TopicDto
                                {
                                    Id = q.Topic.Id,
                                    Name = q.Topic.Name,
                                    SubjectId = q.Topic.SubjectId
                                }
                                : null,
                        Subtopic =
                            q.Subtopic != null
                                ? new SubtopicDto
                                {
                                    Id = q.Subtopic.Id,
                                    Name = q.Subtopic.Name,
                                    TopicId = q.Subtopic.TopicId
                                }
                                : null,
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
        //STEP 1: Check if a topic with the given ID exists under the selected subject.
        var topic = await _context
            .Topics
            .Include(t => t.Subtopics)
            .AsSplitQuery()
            .FirstOrDefaultAsync(
                t => t.Id.Equals(dto.TopicId) && t.SubjectId.Equals(dto.SubjectId)
            );

        if (topic is null)
        {
            _logger.LogWarning(
                "Failed to add question: selected topic {topicId} does not belong to the given subject {subjectId}.",
                dto.TopicId,
                dto.SubtopicId
            );

            throw new KeyNotFoundException(
                $"Unable to add question. Topic with ID '{dto.TopicId}' does not exist under the selected subject."
            );
        }

        //STEP 2: Check if the selected subtopic belongs the selected topic
        if (dto.SubtopicId != null)
        {
            var _ =
                topic.Subtopics.FirstOrDefault(st => st.Id.Equals(dto.SubtopicId))
                ?? throw new KeyNotFoundException(
                    "Unable to add question: selected subtopic does not belong to the selected topic."
                );
        }

        //STEP 2: Get the selected subtopics and make sure they belong to the specified topic
        // var selectedSubtopics = await _context
        //     .Subtopics
        //     .Where(st => dto.SubtopicIds.Contains(st.Id) && st.TopicId == dto.TopicId)
        //     .ToListAsync();
        // if (selectedSubtopics.Count != dto.SubtopicIds.Count)
        // {
        //     _logger.LogWarning(
        //         "Failed to add question: one or more selected subtopics do not exist under a topic with ID {TopicId}",
        //         dto.TopicId
        //     );

        //     throw new InvalidOperationException(
        //         $"Unable to add question: one or more selected subtopics do not exist under a topic with ID {dto.TopicId}"
        //     );
        // }

        //STEP 3: Initialize the question entity with provided details
        Question question =
            new()
            {
                Title = dto.Title,
                ContentText = dto.QuestionText,
                SubjectId = topic.SubjectId,
                TopicId = dto.TopicId,
                SubtopicId = dto.SubtopicId,
                UserId = userId,
                Marks = dto.Marks,
                Status = dto.Status
            };

        //STEP 4: Find each tag by name, or create it if it doesn't exist, then add it to the question.
        // Remove duplicates, ignoring case differences (e.g., "math" and "Math" are treated as the same).
        foreach (string tagName in dto.Tags.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            Tag tag = await _tagService.GetByNameAsync(tagName);
            question.Tags.Add(tag);
        }

        //STEP 5: Finally save the question to the database
        _context.Questions.Add(question);

        await _context.SaveChangesAsync();

        // STEP 6: If the answer was provided, save it as well
        if (dto.AnswerText != null && dto.AnswerHtml != null)
        {
            AddAnswerDto answerDto =
                new() { ContentHtml = dto.AnswerHtml, ContentText = dto.AnswerText };

            await _answerService.AddAsync(userId: userId, questionId: question.Id, answerDto);
        }

        _logger.LogInformation(
            "Successfully created new question by user with ID {UserId}.",
            userId
        );

        return QuestionDto.MapFrom(question);
    }

    /// <summary>
    /// Updates an existing question with new content, topic, subtopics, and tags.
    /// </summary>
    /// <param name="userId">The ID of the user attempting to update the question.</param>
    /// <param name="questionId">The ID of the question to update.</param>
    /// <param name="dto">The updated question data.</param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the question, or topic is not found.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown if the question does not belong to the specified user.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if one or more subtopics are invalid for the selected topic.
    /// </exception>
    public async Task UpdateAsync(int userId, int questionId, UpdateQuestionDto dto)
    {
        //STEP 1: Check if a question with a given ID exists
        var existingQuestion = await _context
            .Questions
            .FirstOrDefaultAsync(q => q.Id.Equals(questionId));
        if (existingQuestion is null)
        {
            _logger.LogWarning(
                "Update failed: question with ID {QuestionId} not found.",
                questionId
            );

            throw new KeyNotFoundException(
                $"Update failed: question with ID '{questionId}' does not exist."
            );
        }

        //STEP 2: Verify that the question belongs to the given user. Prevent updates by other users.
        if (existingQuestion.UserId != userId)
        {
            _logger.LogWarning(
                "Update failed: question with ID {QuestionId} doesn't belong to user with ID {UserId}",
                questionId,
                userId
            );

            throw new UnauthorizedAccessException(
                $"You're not authorized to update this question."
            );
        }

        //STEP 3: Check if a topic with the given ID exists under the selected subject.
        var topic = await _context
            .Topics
            .Include(t => t.Subtopics)
            .AsSplitQuery()
            .FirstOrDefaultAsync(
                t => t.Id.Equals(dto.TopicId) && t.SubjectId.Equals(dto.SubjectId)
            );

        if (topic is null)
        {
            _logger.LogWarning(
                "Failed to add question: selected topic {topicId} does not belong to the given subject {subjectId}.",
                dto.TopicId,
                dto.SubtopicId
            );

            throw new KeyNotFoundException(
                $"Unable to add question. Topic with ID '{dto.TopicId}' does not exist under the selected subject."
            );
        }

        //STEP 4: Get the selected subtopics and make sure they belong to the specified topic
        // var selectedSubtopics = await _context
        //     .Subtopics
        //     .Where(st => dto.SubtopicIds.Contains(st.Id) && st.TopicId == dto.TopicId)
        //     .ToListAsync();
        // if (selectedSubtopics.Count != dto.SubtopicIds.Count)
        // {
        //     _logger.LogWarning(
        //         "Failed to update question: one or more selected subtopics do not exist under a topic with ID {TopicId}",
        //         dto.TopicId
        //     );

        //     throw new InvalidOperationException(
        //         $"Unable to update question: one or more selected subtopics do not exist under a topic with ID {dto.TopicId}"
        //     );
        // }

        //STEP 4: Check if the selected subtopic belongs the selected topic
        if (dto.SubtopicId != null)
        {
            var _ =
                topic.Subtopics.FirstOrDefault(st => st.Id.Equals(dto.SubtopicId))
                ?? throw new KeyNotFoundException(
                    "Unable to add question: selected subtopic does not belong to the selected topic."
                );
        }

        //STEP 5: Update the existing question entity with provided details
        existingQuestion.Title = dto.Title;
        existingQuestion.ContentText = dto.QuestionText;
        existingQuestion.ContentHtml = dto.QuestionHtml;
        existingQuestion.TopicId = dto.TopicId;
        existingQuestion.SubjectId = topic.SubjectId;
        existingQuestion.SubtopicId = dto.SubtopicId;
        existingQuestion.Marks = dto.Marks;
        existingQuestion.Status = dto.Status;

        //clear subtopics before adding new one
        // existingQuestion.Subtopics.Clear();
        // //add new subtopics
        // existingQuestion.Subtopics.AddRange(selectedSubtopics);

        //clear tags before adding new ones
        existingQuestion.Tags.Clear();

        //STEP 6: Find each tag by name, or create it if it doesn't exist, then add it to the question.
        // Go through each tag name provided in the request.
        // Remove duplicates, ignoring case differences (e.g., "math" and "Math" are treated as the same).
        foreach (string tagName in dto.Tags.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            Tag tag = await _tagService.GetByNameAsync(tagName);

            existingQuestion.Tags.Add(tag);
        }

        //STEP 7: Finally persist the changes to the database
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
