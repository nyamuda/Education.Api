using System.Text.Json;
using Education.Api.Data;
using Education.Api.Dtos.Curriculums;
using Education.Api.Exceptions;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Curriculums;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Services.Implementations.Curriculums;

public class CurriculumService(
    ApplicationDbContext context,
    ILogger<CurriculumService> logger,
    IWebHostEnvironment env
) : ICurriculumService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<CurriculumService> _logger = logger;
    private readonly IWebHostEnvironment _env = env;
    private static readonly JsonSerializerOptions _jsonOptions =
        new() { PropertyNameCaseInsensitive = true };

    //Gets a curriculum with a given ID
    public async Task<CurriculumDto> GetByIdAsync(int id)
    {
        return await _context
                .Curriculums
                .AsNoTracking()
                .Select(
                    c =>
                        new CurriculumDto
                        {
                            Id = c.Id,
                            Name = c.Name,
                            CreatedAt = c.CreatedAt
                        }
                )
                .FirstOrDefaultAsync(c => c.Id.Equals(id))
            ?? throw new KeyNotFoundException($"Curriculum with ID '{id}' does not exist.");
    }

    /// <summary>
    /// Retrieves a paginated list of curriculums.
    /// </summary>
    /// <param name="page">The current page number.</param>
    /// <param name="pageSize">The number of items to include per page.</param>
    /// <returns>
    /// A <see cref="PageInfo{CurriculumDto}"/> containing the list of curriculums for the specified page,
    /// along with pagination metadata such as page number, page size, and whether more items are available.
    /// </returns>
    public async Task<PageInfo<CurriculumDto>> GetAsync(int page, int pageSize)
    {
        var query = _context.Curriculums.OrderByDescending(c => c.CreatedAt).AsQueryable();

        List<CurriculumDto> items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .Select(
                c =>
                    new CurriculumDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        CreatedAt = c.CreatedAt
                    }
            )
            .ToListAsync();

        //pagination info
        int totalItems = await query.CountAsync();
        bool hasMore = totalItems > page * pageSize;

        return new PageInfo<CurriculumDto>
        {
            Page = page,
            PageSize = pageSize,
            HasMore = hasMore,
            Items = items
        };
    }

    /// <summary>
    /// Adds a new curriculum to the database after verifying that its name is unique.
    /// </summary>
    /// <param name="dto">The DTO containing the curriculum's name.</param>
    /// <returns>
    /// A <see cref="CurriculumDto"/> representing the newly created curriculum.
    /// </returns>
    /// <exception cref="ConflictException">
    /// Thrown if a curriculum with the same name already exists (case-insensitive).
    /// </exception>
    public async Task<CurriculumDto> AddAsync(AddCurriculumDto dto)
    {
        //Curriculum name is unique.
        //Check if there isn't already another curriculum with the given name
        bool alreadyExists = await _context
            .Curriculums
            .AnyAsync(c => c.Name.ToLower().Equals(dto.Name.ToLower()));

        if (alreadyExists)
        {
            _logger.LogWarning(
                "Failed to create curriculum. Curriculum with name {CurriculumName} already exists.",
                dto.Name
            );
            throw new ConflictException(
                $"Cannot add curriculum. Curriculum with name '{dto.Name}' already exists."
            );
        }

        //add the new curriculum to the database
        Curriculum curriculum = new() { Name = dto.Name };

        await _context.Curriculums.AddAsync(curriculum);

        _logger.LogInformation("New curriculum created with name {CurriculumName}", dto.Name);

        return CurriculumDto.MapFrom(curriculum);
    }

    /// <summary>
    /// Updates an existing curriculum with a given ID.
    /// </summary>
    /// <param name="id">The ID of the curriculum to update.</param>
    /// <param name="dto">The DTO containing the updated curriculum</param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if no curriculum with the specified ID exists.
    /// </exception>
    /// <exception cref="ConflictException">
    /// Thrown if another curriculum with the same name already exists (case-insensitive).
    /// </exception>

    public async Task UpdateAsync(int id, UpdateCurriculumDto dto)
    {
        var curriculum =
            await _context.Curriculums.FirstOrDefaultAsync(c => c.Id.Equals(id))
            ?? throw new KeyNotFoundException(
                $"Update failed: curriculum with ID '{id}' does not exist."
            );

        //curriculum name is unique.
        //check if there isn't already an existing curriculum with the new updated name
        bool alreadyExists = await _context
            .Curriculums
            .AnyAsync(c => c.Name.ToLower().Equals(dto.Name.ToLower()) && c.Id != id);

        if (alreadyExists)
        {
            _logger.LogWarning(
                "Update failed: curriculum with name {CurriculumName} already exists.",
                dto.Name
            );
            throw new ConflictException(
                $"Update failed: curriculum with name '{dto.Name}' already exists."
            );
        }

        curriculum.Name = dto.Name;

        _logger.LogInformation("Successfully updated curriculum: {CurriculumId}", id);

        await _context.SaveChangesAsync();
    }

    //Deletes a curriculum with a given ID
    public async Task DeleteAsync(int id)
    {
        var curriculum =
            await _context.Curriculums.FirstOrDefaultAsync(c => c.Id.Equals(id))
            ?? throw new KeyNotFoundException(
                $"Delete failed: curriculum with ID '{id}' does not exist."
            );

        _context.Curriculums.Remove(curriculum);

        await _context.SaveChangesAsync();

        _logger.LogInformation("Successfully deleted curriculum: {CurriculumId}", id);
    }

    /// <summary>
    /// Loads and deserializes curriculums from a JSON a file.
    /// </summary>
    /// <returns>A list of the deserialized curriculums.</returns>
    /// <exception cref="FileNotFoundException">
    /// Thrown when the JSON file cannot be found at the expected path.
    /// </exception>
    /// <exception cref="Exception">
    /// Thrown when the JSON content cannot be deserialized into the expected list of curriculums.
    /// </exception>
    public async Task<List<Curriculum>> DeserializeCurriculumsFromFileAsync()
    {
        string fileName = "curriculums.json";
        //first, get the full file path
        var filePath = Path.Combine(_env.ContentRootPath, "Data", "Curriculums", fileName);

        //next, check if the file exists
        if (!File.Exists(filePath))
        {
            _logger.LogWarning(
                "Deserialization failed. Curriculums JSON file {fileName} not found.",
                filePath
            );

            throw new FileNotFoundException(
                $"Unable to deserialize curriculums. File {fileName} could not be found."
            );
        }
        //read the json file
        var json = await File.ReadAllTextAsync(filePath);

        //finally, deserialize the JSON
        var curriculums =
            JsonSerializer.Deserialize<List<Curriculum>>(json, _jsonOptions)
            ?? throw new Exception(
                $"Curriculum deserialization failed: unable to deserialize the '{fileName}' file."
            );

        return curriculums;
    }
}
