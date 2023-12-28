using Microsoft.AspNetCore.Mvc;

namespace MoviesAPI.Controllers;
[Route("api/genres")]
[ApiController]
public class GenresController : CustomBaseController {
    public GenresController(ApplicationDbContext dbContext, IMapper mapper):
        base(dbContext, mapper) {
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
        return await Post<GenreCreationDTO, Genre, GenreDTO>(genreCreationDTO, "obtenerGenero");
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, [FromBody] GenreCreationDTO genreCreationDTO) {
        return await Put<GenreCreationDTO, Genre>(id, genreCreationDTO);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id) {
        return await Delete<Genre>(id);
    }
}