using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace MoviesAPI.Controllers; 
[Route("api/cinemas")]
[ApiController]
public class CinemasController : CustomBaseController {
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;

    public CinemasController(ApplicationDbContext context, IMapper mapper): base(context, mapper) {
        this.context = context;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<CinemaDTO>>> Get() {
        return await Get<Cinema, CinemaDTO>();
    }

    [HttpGet("{id:int}", Name = "obtenerCinema")]
    public async Task<ActionResult<CinemaDTO>> Get(int id) {
        return await Get<Cinema, CinemaDTO>(id);
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