using Microsoft.AspNetCore.Mvc;

#nullable disable

namespace MoviesAPI.Controllers; 
[Route("api/cinemas")]
[ApiController]
public class CinemasController : CustomBaseController {
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;
    private readonly GeometryFactory geometryFactory;

    public CinemasController(ApplicationDbContext context, IMapper mapper,
                GeometryFactory geometryFactory): base(context, mapper) {
        this.context = context;
        this.mapper = mapper;
        this.geometryFactory = geometryFactory;
    }

    [HttpGet]
    public async Task<ActionResult<List<CinemaDTO>>> Get() {
        return await Get<Cinema, CinemaDTO>();
    }

    [HttpGet("{id:int}", Name = "obtenerCinema")]
    public async Task<ActionResult<CinemaDTO>> Get(int id) {
        return await Get<Cinema, CinemaDTO>(id);
    }

    [HttpGet("Cercanos")]
    public async Task<ActionResult<List<NearCinemaDTO>>> Cercanos(
                [FromQuery] NearCinemaFilterDTO filter) {
        var userLocation = geometryFactory.CreatePoint(new Coordinate(filter.Longitude, filter.Latitude));

        var cinemas = await context.Cinemas
                    .OrderBy(x => x.Location.Distance(userLocation))
                    .Where(x => x.Location.IsWithinDistance(userLocation, filter.DistanceInKms * 1000))
                    .Select(x => new NearCinemaDTO {
                        Id= x.Id,
                        Name = x.Name,
                        Latitude = x.Location.X,
                        Longitude = x.Location.Y,
                        DistanceInMeters = Math.Round(x.Location.Distance(userLocation))
                    }).ToListAsync();

        return cinemas;
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] CinemaCreationDTO cinemaCreationDTO) {
        return await Post<CinemaCreationDTO, Cinema, CinemaDTO>(cinemaCreationDTO, "obtenerCinema");
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, [FromBody] CinemaCreationDTO cinemaCreationDTO) {
        return await Put<CinemaCreationDTO, Cinema>(id, cinemaCreationDTO);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id) {
        return await Delete<Cinema>(id);
    }
}