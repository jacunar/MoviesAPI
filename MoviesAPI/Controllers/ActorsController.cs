using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MoviesAPI.Controllers;
[Route("api/actors")]
[ApiController]
public class ActorsController : ControllerBase {
    private readonly ApplicationDbContext dbContext;
    private readonly IMapper mapper;
    private readonly IFileStorage fileStorage;
    private readonly string container = "favorites";

    public ActorsController(ApplicationDbContext dbContext, IMapper mapper,
                    IFileStorage fileStorage) {
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.fileStorage = fileStorage;
    }

    [HttpGet]
    public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginationDTO paginationDTO) {
        var queryable = dbContext.Actors.AsQueryable();
        await HttpContext.InsertParameterPagination(queryable, paginationDTO.RecordsPerPage);
        var entities = await queryable.Page(paginationDTO).ToListAsync();
        return mapper.Map<List<ActorDTO>>(entities);
    }

    [HttpGet("{id:int}", Name = "obtenerActor")]
    public async Task<ActionResult<ActorDTO>> Get(int id) {
        var entity = await dbContext.Actors.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
            return NotFound();

        return mapper.Map<ActorDTO>(entity);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromForm] ActorCreationDTO actorCreationDTO) {
        var entity = mapper.Map<Actor>(actorCreationDTO);

        if (actorCreationDTO.Picture != null) {
            using (var memoryStream = new MemoryStream()) {
                await actorCreationDTO.Picture.CopyToAsync(memoryStream);
                var content = memoryStream.ToArray();
                var extension = Path.GetExtension(actorCreationDTO.Picture.FileName);
                entity.Picture = await fileStorage.SaveFile(content, extension, container, actorCreationDTO.Picture.ContentType);
            }
        }

        dbContext.Add(entity);
        await dbContext.SaveChangesAsync();
        var dto = mapper.Map<ActorDTO>(entity);
        return new CreatedAtRouteResult("obtenerActor", new { id = entity.Id }, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, [FromForm] ActorCreationDTO actorCreationDTO) {
        var actorDb = await dbContext.Actors.FirstOrDefaultAsync(x => x.Id == id);
        if (actorDb == null) return NotFound();

        actorDb = mapper.Map(actorCreationDTO, actorDb);
        if (actorCreationDTO.Picture != null) {
            using (var memoryStream = new MemoryStream()) {
                await actorCreationDTO.Picture.CopyToAsync(memoryStream);
                var content = memoryStream.ToArray();
                var extension = Path.GetExtension(actorCreationDTO.Picture.FileName);
                actorDb.Picture = await fileStorage.EditFile(content, extension, container,
                        actorDb.Picture, actorCreationDTO.Picture.ContentType);
            }
        }

        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<ActorPatchDTO> patchDocument) {
        if (patchDocument is null)
            return BadRequest();

        var entidadDB = await dbContext.Actors.FirstOrDefaultAsync(x => x.Id == id);

        if (entidadDB is null)
            return NotFound();

        var entidadDTO = mapper.Map<ActorPatchDTO>(entidadDB);
        patchDocument.ApplyTo(entidadDTO, ModelState);

        var isValid = TryValidateModel(entidadDTO);
        if (!isValid)
            return BadRequest(ModelState);

        mapper.Map(entidadDTO, entidadDB);
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