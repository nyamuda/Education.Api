using Education.Api.Data;
using Education.Api.Dtos.Curriculums;
using Education.Api.Dtos.ExamBoards;
using Education.Api.Models;
using Education.Api.Services.Abstractions.ExamBoards;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Services.Implementations.ExamBoards;

public class ExamBoardService(ApplicationDbContext context) : IExamBoardService
{
    private readonly ApplicationDbContext _context = context;

    //Gets an exam board with a given ID
    public async Task<ExamBoardDto> GetByIdAsync(int id)
    {
        return await _context
                .ExamBoards
                .AsNoTracking()
                .Select(
                    eb =>
                        new ExamBoardDto
                        {
                            Id = eb.Id,
                            Name = eb.Name,
                            CurriculumId = eb.CurriculumId,
                            Curriculum =
                                eb.Curriculum != null
                                    ? new CurriculumDto
                                    {
                                        Id = eb.Curriculum.Id,
                                        Name = eb.Curriculum.Name
                                    }
                                    : null,
                            CreatedAt = eb.CreatedAt
                        }
                )
                .FirstOrDefaultAsync(c => c.Id.Equals(id))
            ?? throw new KeyNotFoundException($"ExamBoard with ID '{id}' does not exist.");
    }

    /// <summary>
    /// Retrieves a paginated list of exam boards.
    /// </summary>
    /// <param name="page">The current page number.</param>
    /// <param name="pageSize">The number of items to include per page.</param>
    /// <returns>
    /// A <see cref="PageInfo{ExamBoardDto}"/> containing the list of exam boards for the specified page,
    /// along with pagination metadata such as page number, page size, and whether more items are available.
    /// </returns>
    public async Task<PageInfo<ExamBoardDto>> GetAsync(int page, int pageSize)
    {
        var query = _context.ExamBoards.OrderByDescending(eb => eb.CreatedAt).AsQueryable();

        List<ExamBoardDto> items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(
                eb =>
                    new ExamBoardDto
                    {
                        Id = eb.Id,
                        Name = eb.Name,
                        CurriculumId = eb.CurriculumId,
                        CreatedAt = eb.CreatedAt
                    }
            )
            .ToListAsync();

        //pagination info
        int totalItems = await query.CountAsync();
        bool hasMore = totalItems > page * pageSize;

        return new PageInfo<ExamBoardDto>
        {
            Page = page,
            PageSize = pageSize,
            HasMore = hasMore,
            Items = items
        };
    }

    /// <summary>
    /// Adds a new exam board to the database after validating its uniqueness and associated curriculum.
    /// </summary>
    /// <param name="dto">The DTO containing the exam board's name and related curriculum ID.</param>
    /// <returns>
    /// An <see cref="ExamBoardDto"/> representing the newly created exam board.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if an exam board with the same name already exists (case-insensitive).
    /// </exception>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the specified curriculum ID does not exist in the database.
    /// </exception>
    public async Task<ExamBoardDto> AddAsync(AddExamBoardDto dto)
    {
        //Exam board name is unique.
        //Check if there isn't already another exam board with the given name.
        bool alreadyExists = await _context
            .ExamBoards
            .AnyAsync(eb => eb.Name.ToLower().Equals(dto.Name.ToLower()));
        if (alreadyExists)
        {
            throw new InvalidOperationException(
                $"Exam board with name '{dto.Name}' already exists."
            );
        }
        //check if the curriculum with the given ID exists
        var _ =
            await _context
                .Curriculums
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id.Equals(dto.CurriculumId))
            ?? throw new KeyNotFoundException(
                $"Curriculum with ID '{dto.CurriculumId}' does not exist."
            );

        //add the new exam board to the database
        ExamBoard examBoard = new() { Name = dto.Name, CurriculumId = dto.CurriculumId };
        await _context.ExamBoards.AddAsync(examBoard);

        return ExamBoardDto.MapFrom(examBoard);
    }

    //Updates a exam board with a given ID
    public async Task UpdateAsync(int id, UpdateExamBoardDto dto)
    {
        var examBoard =
            await _context.ExamBoards.FirstOrDefaultAsync(eb => eb.Id.Equals(id))
            ?? throw new KeyNotFoundException($"ExamBoard with ID '{id}' does not exist.");

        examBoard.Name = dto.Name;

        await _context.SaveChangesAsync();
    }

    //Deletes a exam board with a given ID
    public async Task DeleteAsync(int id)
    {
        var examBoard =
            await _context.ExamBoards.FirstOrDefaultAsync(eb => eb.Id.Equals(id))
            ?? throw new KeyNotFoundException($"ExamBoard with ID '{id}' does not exist.");

        _context.ExamBoards.Remove(examBoard);

        await _context.SaveChangesAsync();
    }
}
