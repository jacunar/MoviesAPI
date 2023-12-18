using Microsoft.AspNetCore.Mvc;

namespace MoviesAPI.Controllers;
[Route("api/actors")]
[ApiController]
public class ActorsController : ControllerBase {
    private readonly ApplicationDbContext dbContext;
    private readonly IMapper mapper;

    public ActorsController(ApplicationDbContext dbContext, IMapper mapper) {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<ActorDTO>>> Get() {
        var entities = await dbContext.Actors.ToListAsync();
        return mapper.Map<List<ActorDTO>>(entities);
    }

    [HttpGet("{id:int}", Name = "obtenetActor")]
    public async Task<ActionResult<ActorDTO>> Get(int id) {
        var entity = await dbContext.Actors.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
            return NotFound();

        return mapper.Map<ActorDTO>(entity);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromForm] ActorCreationDTO actorCreationDTO) {
        var entity = mapper.Map<Actor>(actorCreationDTO);
        dbContext.Add(entity);
        await dbContext.SaveChangesAsync();
        var dto = mapper.Map<ActorDTO>(entity);
        return new CreatedAtRouteResult("obtenerActor", new { id = entity.Id }, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, [FromForm] ActorCreationDTO actorCreationDTO) {
        var entity = mapper.Map<Actor>(actorCreationDTO);
        entity.Id = id;
        dbContext.Entry(entity).State = EntityState.Modified;
        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id) {
        var existe = await dbContext.Actors.AnyAsync(x => x.Id == id);
        if (!existe)
            return NotFound();

        dbContext.Remove(new Actor() { Id = id });
        await dbContext.SaveChangesAsync();
        return NoContent();
    }
}