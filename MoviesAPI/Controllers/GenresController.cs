using Azure.Core;
using Microsoft.AspNetCore.Mvc;

namespace MoviesAPI.Controllers;
[Route("api/genres")]
[ApiController]
public class GenresController : CustomBaseController {
    private readonly ApplicationDbContext dbContext;
    private readonly IMapper mapper;

    public GenresController(ApplicationDbContext dbContext, IMapper mapper):
        base(dbContext, mapper) {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<GenreDTO>>> Get() {
        return await Get<Genre, GenreDTO>();
    }

    [HttpGet("{id:int}", Name = "obtenerGenero")]
    public async Task<ActionResult<GenreDTO>> Get(int id) {
        return await Get<Genre, GenreDTO>(id);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] GenreCreationDTO genreCreationDTO) {
        var entidad = mapper.Map<Genre>(genreCreationDTO);
        dbContext.Add(entidad);
        await dbContext.SaveChangesAsync();
        var genreDTO = mapper.Map<GenreDTO>(entidad);
        return new CreatedAtRouteResult("obtenerGenero", new { id = genreDTO.Id }, genreDTO);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, [FromBody] GenreCreationDTO genreCreationDTO) {
        var entity = mapper.Map<Genre>(genreCreationDTO);
        entity.Id = id;
        dbContext.Entry(entity).State = EntityState.Modified;
        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id) {
        var existe = await dbContext.Genres.AnyAsync(x => x.Id == id);
        if (!existe)
            return NotFound();

        dbContext.Remove(new Genre() { Id = id });
        await dbContext.SaveChangesAsync();
        return NoContent();
    }
}