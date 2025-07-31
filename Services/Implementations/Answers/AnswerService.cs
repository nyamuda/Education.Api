using Education.Api.Data;
using Education.Api.Dtos.Answers;
using Education.Api.Dtos.Users;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Answers;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Services.Implementations.Answers;

public class AnswerService(ApplicationDbContext context, ILogger<AnswerService> logger)
    : IAnswerService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<AnswerService> _logger = logger;

    //Gets a answer with a given ID
    public async Task<AnswerDto> GetByIdAsync(int id)
    {
        return await _context
                .Answers
                .AsNoTracking()
                .Select(
                    a =>
                        new AnswerDto
                        {
                            Id = a.Id,
                            Content = a.Content,
                            QuestionId = a.QuestionId,
                            UserId = a.UserId,
                            User =
                                a.User != null
                                    ? new UserDto { Id = a.User.Id, Username = a.User.Username, }
                                    : null,
                            CreatedAt = a.CreatedAt,
                            UpdatedAt = a.UpdatedAt
                        }
                )
                .FirstOrDefaultAsync(a => a.Id.Equals(id))
            ?? throw new KeyNotFoundException($"Answer with ID '{id}' does not exist.");
    }

    /// <summary>
    /// Retrieves a paginated list of answers for a question with a given ID.
    /// </summary>
    /// <param name="questionId">The ID of the question whose answers are to be retrieved.</param>
    /// <param name="page">The current page number.</param>
    /// <param name="pageSize">The number of items to include per page.</param>
    /// <returns>
    /// A <see cref="PageInfo{AnswerDto}"/> containing the list of answers for the specified page,
    /// along with pagination metadata such as page number, page size, and whether more items are available.
    /// </returns>
    public async Task<PageInfo<AnswerDto>> GetAsync(int questionId, int page, int pageSize)
    {
        var query = _context
            .Answers
            .Where(a => a.QuestionId == questionId)
            .OrderByDescending(a => a.CreatedAt)
            .AsQueryable();

        List<AnswerDto> items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .Select(
                a =>
                    new AnswerDto
                    {
                        Id = a.Id,
                        Content = a.Content,
                        QuestionId = a.QuestionId,
                        UserId = a.UserId,
                        User =
                            a.User != null
                                ? new UserDto { Id = a.User.Id, Username = a.User.Username, }
                                : null,
                        CreatedAt = a.CreatedAt,
                        UpdatedAt = a.UpdatedAt
                    }
            )
            .ToListAsync();

        //pagination info
        int totalItems = await query.CountAsync();
        bool hasMore = totalItems > page * pageSize;

        return new PageInfo<AnswerDto>
        {
            Page = page,
            PageSize = pageSize,
            HasMore = hasMore,
            Items = items
        };
    }

    /// <summary>
    /// Adds a new answer to the database.
    /// </summary>
    /// <param name="dto">The DTO containing the answer's data.</param>
    /// <returns>
    /// A <see cref="AnswerDto"/> representing the newly created answer.
    /// </returns>
    /// <exception cref="ConflictException">
    /// Thrown if a answer with the same name already exists (case-insensitive).
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if one or more of the provided exam board IDs do not exist.
    /// </exception>
    public async Task<AnswerDto> AddAsync(AddAnswerDto dto)
    {
        //Answer name is unique.
        //Check if there isn't already another answer with the given name (case-insensitive)
        bool alreadyExists = await _context
            .Answers
            .AnyAsync(s => s.Name.ToLower().Equals(dto.Name.ToLower()));

        if (alreadyExists)
        {
            throw new ConflictException($"Answer with name '{dto.Name}' already exists.");
        }

        //get the the selected exam boards for the answer
        var selectedExamBoards = await _context
            .ExamBoards
            .Where(eb => dto.ExamBoardIds.Contains(eb.Id))
            .ToListAsync();

        //Make sure all the selected exam boards exist
        if (selectedExamBoards.Count != dto.ExamBoardIds.Count)
        {
            throw new InvalidOperationException("One or more selected exam boards do not exist.");
        }

        //add the new answer to the database
        Answer answer = new() { Name = dto.Name };
        answer.ExamBoards.AddRange(selectedExamBoards);

        await _context.Answers.AddAsync(answer);

        return AnswerDto.MapFrom(answer);
    }

    /// <summary>
    /// Updates an existing answer with a given ID.
    /// </summary>
    /// <param name="id">The ID of the answer to update.</param>
    /// <param name="dto">The DTO containing the updated answer</param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if no answer with the specified ID exists.
    /// </exception>
    /// <exception cref="ConflictException">
    /// Thrown if another answer with the same name already exists (case-insensitive).
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if one or more of the provided exam board IDs do not exist.
    /// </exception>

    public async Task UpdateAsync(int id, UpdateAnswerDto dto)
    {
        var answer =
            await _context.Answers.FirstOrDefaultAsync(s => s.Id.Equals(id))
            ?? throw new KeyNotFoundException($"Answer with ID '{id}' does not exist.");

        //answer name is unique.
        //check if there isn't already an existing answer with the new updated name
        bool alreadyExists = await _context
            .Answers
            .AnyAsync(s => s.Name.ToLower().Equals(dto.Name.ToLower()) && s.Id != id);
        if (alreadyExists)
        {
            throw new ConflictException($"A answer with name '{dto.Name}' already exists.");
        }

        //get the the selected exam boards for the answer
        var selectedExamBoards = await _context
            .ExamBoards
            .Where(eb => dto.ExamBoardIds.Contains(eb.Id))
            .ToListAsync();

        //Make sure all the selected exam boards exist
        if (selectedExamBoards.Count != dto.ExamBoardIds.Count)
        {
            throw new InvalidOperationException("One or more selected exam boards do not exist.");
        }

        answer.Name = dto.Name;
        answer.ExamBoards.AddRange(selectedExamBoards);

        await _context.SaveChangesAsync();
    }

    //Deletes a answer with a given ID
    public async Task DeleteAsync(int id)
    {
        var answer =
            await _context.Answers.FirstOrDefaultAsync(s => s.Id.Equals(id))
            ?? throw new KeyNotFoundException($"Answer with ID '{id}' does not exist.");

        _context.Answers.Remove(answer);

        await _context.SaveChangesAsync();
    }
}
