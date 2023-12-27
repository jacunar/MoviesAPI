using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Entidades;

namespace MoviesAPI.Controllers;
[Route("api/movies")]
[ApiController]
public class MoviesController : ControllerBase {
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;
    private readonly IFileStorage fileStorage;
    private readonly string container = "favorites";

    public MoviesController(ApplicationDbContext context, IMapper mapper, IFileStorage fileStorage) {
        this.context = context;
        this.mapper = mapper;
        this.fileStorage = fileStorage;
    }

    [HttpGet]
    public async Task<ActionResult<List<MovieDTO>>> Get() {
        var movies = await context.Movies.ToListAsync();
        return mapper.Map<List<MovieDTO>>(movies);
    }

    [HttpGet("{id}", Name = "obtenerPelicula")]
    public async Task<ActionResult<MovieDTO>> Get(int id) {
        var movie = await context.Movies.FirstOrDefaultAsync(x => x.Id == id);
        if (movie is null)
            return NotFound();

        return mapper.Map<MovieDTO>(movie);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromForm] MovieCreationDTO movieCreationDTO) {
        var movie = mapper.Map<Movie>(movieCreationDTO);

        if (movieCreationDTO.Poster != null) {
            using var memoryStream = new MemoryStream();
            var contenido = memoryStream.ToArray();
            var extension = Path.GetExtension(movieCreationDTO.Poster.FileName);
            movie.Poster = await fileStorage.SaveFile(contenido, extension, container, movieCreationDTO.Poster.ContentType);
        }

        AsignActorsOrder(movie);
        context.Add(movie);
        await context.SaveChangesAsync();
        var movieDTO = mapper.Map<MovieDTO>(movie);
        return new CreatedAtRouteResult("obtenerPelicula", new { id = movie.Id }, movieDTO);
    }

    [HttpPut]
    public async Task<ActionResult> Put(int id, [FromForm] MovieCreationDTO movieCreationDTO) {
        var movieDB = await context.Movies
            .Include(x => x.MoviesActors)
            .Include(x => x.MoviesGenres)
            .FirstOrDefaultAsync(x =>x.Id == id);

        if (movieDB == null) 
            return NotFound();

        movieDB = mapper.Map(movieCreationDTO, movieDB);
        if (movieCreationDTO.Poster != null) {
            using var memoryStream = new MemoryStream();
            var contenido = memoryStream.ToArray();
            var extension = Path.GetExtension(movieCreationDTO.Poster.FileName);
            movieDB.Poster = await fileStorage.EditFile(contenido, extension, container, 
                    movieDB.Poster, movieCreationDTO.Poster.ContentType);
        }

        AsignActorsOrder(movieDB);
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<MoviePatchDTO> patchDocument) {
        if (patchDocument is null)
            return BadRequest();

        var entidadDB = await context.Movies.FirstOrDefaultAsync(x => x.Id == id);

        if (entidadDB is null)
            return NotFound();

        var entidadDTO = mapper.Map<MoviePatchDTO>(entidadDB);
        patchDocument.ApplyTo(entidadDTO, ModelState);

        var isValid = TryValidateModel(entidadDTO);
        if (!isValid)
            return BadRequest(ModelState);

        mapper.Map(entidadDTO, entidadDB);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id) {
        var existe = await context.Movies.AnyAsync(x => x.Id == id);
        if (!existe)
            return NotFound();

        context.Remove(new Movie() { Id = id });
        await context.SaveChangesAsync();
        return NoContent();
    }

    private void AsignActorsOrder(Movie movie) {
        if (movie != null) {
            for (int i = 0; i < movie.MoviesActors.Count; i++) {
                movie.MoviesActors[i].Orden = i;
            }
        }
    }
}