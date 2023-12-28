using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;

namespace MoviesAPI.Controllers;
[Route("api/movies")]
[ApiController]
public class MoviesController : CustomBaseController {
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;
    private readonly IFileStorage fileStorage;
    private readonly ILogger<MoviesController> logger;
    private readonly string container = "favorites";

    public MoviesController(ApplicationDbContext context, IMapper mapper, 
                IFileStorage fileStorage, ILogger<MoviesController> logger): base(context, mapper) {
        this.context = context;
        this.mapper = mapper;
        this.fileStorage = fileStorage;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<MoviesIndexDTO>> Get() {
        int top = 5;
        DateTime today = DateTime.Today;

        var proximosEstrenos = await context.Movies
                .Where(x => x.Premiere > today)
                .OrderBy(x => x.Premiere)
                .Take(top)
                .ToListAsync();

        var enCines = await context.Movies
                .Where(x => x.OnCimena)
                .Take(top)
                .ToListAsync();

        var resultado = new MoviesIndexDTO();
        resultado.EnCines = mapper.Map<List<MovieDTO>>(enCines);
        resultado.FuturosEstrenos = mapper.Map<List<MovieDTO>>(proximosEstrenos);

        return resultado;
    }

    [HttpGet("filtrar")]
    public async Task<ActionResult<List<MovieDTO>>> Filter([FromQuery] FilterMovieDTO filterMovieDTO) {
        var moviesQueryable = context.Movies.AsQueryable();

        if (!string.IsNullOrEmpty(filterMovieDTO.Title))
            moviesQueryable = moviesQueryable.Where(x => x.Title.Contains(filterMovieDTO.Title));

        if (filterMovieDTO.OnCinemas)
            moviesQueryable = moviesQueryable.Where(x => x.OnCimena);

        if (filterMovieDTO.ProximosEstrenos) {
            var today = DateTime.Today;
            moviesQueryable = moviesQueryable.Where(x => x.Premiere > today);
        }

        if (filterMovieDTO.GenreId > 0) { 
            moviesQueryable = moviesQueryable.Where(x => x.MoviesGenres.Select(g => g.GenreId)
                .Contains(filterMovieDTO.GenreId));
        }

        if (!string.IsNullOrEmpty(filterMovieDTO.FieldToOrder)) {
            string orderType = filterMovieDTO.Ascending ? "ascending" : "descending";

            try {
                moviesQueryable = moviesQueryable.OrderBy($"{filterMovieDTO.FieldToOrder} {orderType}");
            } catch(Exception ex) {
                logger.LogError(ex.Message, ex);
            }
        }

        await HttpContext.InsertParameterPagination(moviesQueryable, filterMovieDTO.RecordsPerPage);

        var movies = await moviesQueryable.Page(filterMovieDTO.Pagination).ToListAsync();
        return mapper.Map<List<MovieDTO>>(movies);
    }

    [HttpGet("{id}", Name = "obtenerPelicula")]
    public async Task<ActionResult<MovieDetailDTO>> Get(int id) {
        var movie = await context.Movies
                .Include(x => x.MoviesActors).ThenInclude(x => x.Actor)
                .Include(x => x.MoviesGenres).ThenInclude(x => x.Genre)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (movie is null)
            return NotFound();

        movie.MoviesActors = movie.MoviesActors.OrderBy(x => x.Orden).ToList();
        return mapper.Map<MovieDetailDTO>(movie);
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
        return await Patch<Movie, MoviePatchDTO>(id, patchDocument);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id) {
        return await Delete<Movie>(id); 
    }

    private void AsignActorsOrder(Movie movie) {
        if (movie != null) {
            for (int i = 0; i < movie.MoviesActors.Count; i++) {
                movie.MoviesActors[i].Orden = i;
            }
        }
    }
}