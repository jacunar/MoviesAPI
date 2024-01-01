using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;

namespace MoviesAPI.Controllers;

public class CustomBaseController : ControllerBase {
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;

    public CustomBaseController(ApplicationDbContext context, IMapper mapper) {
        this.context = context;
        this.mapper = mapper;
    }

    protected async Task<List<TDTO>> Get<TEntidad, TDTO>() where TEntidad : class {
        var entities = await context.Set<TEntidad>().AsNoTracking().ToListAsync();
        var dtos = mapper.Map<List<TDTO>>(entities);
        return dtos;
    }

    protected async Task<ActionResult<TDTO>> Get<TEntidad, TDTO>(int id) where TEntidad : class, IId {
        var entity = await context.Set<TEntidad>().AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

        if (entity is null)
            return NotFound();

        return mapper.Map<TDTO>(entity);
    }

    protected async Task<List<TDTO>> Get<TEntidad, TDTO>(PaginationDTO paginacionDTO,
            IQueryable<TEntidad> queryable)
            where TEntidad : class {
        await HttpContext.InsertParameterPagination(queryable, paginacionDTO.RecordsPerPage);
        var entidades = await queryable.Page(paginacionDTO).ToListAsync();
        return mapper.Map<List<TDTO>>(entidades);
    }

    protected async Task<List<TDTO>> Get<TEntidad, TDTO>(PaginationDTO paginationDTO) where TEntidad: class {
        var queryable = context.Set<TEntidad>().AsQueryable();
        return await Get<TEntidad, TDTO>(paginationDTO, queryable);
    }

    protected async Task<ActionResult> Post<TCreacion, TEntidad, TLectura>(TCreacion creacionDTO, string path) where TEntidad : class, IId {
        var entity = mapper.Map<TEntidad>(creacionDTO);
        context.Add(entity);
        await context.SaveChangesAsync();
        var dtoLectura = mapper.Map<TLectura>(entity);

        return new CreatedAtRouteResult(path, new { id = entity.Id }, dtoLectura);
    }

    protected async Task<ActionResult> Put<TCreacion, TEntidad>(int id, TCreacion creacionDTO) where TEntidad : class, IId {
        var entity = mapper.Map<TEntidad>(creacionDTO);
        entity.Id = id;
        context.Entry(entity).State = EntityState.Modified;
        await context.SaveChangesAsync();
        return NoContent();
    }

    protected async Task<ActionResult> Patch<TEntidad, TDTO>(int id, JsonPatchDocument<TDTO> patchDocument)
        where TDTO: class
        where TEntidad: class, IId {
        if (patchDocument is null)
            return BadRequest();

        var entidadDB = await context.Set<TEntidad>().FirstOrDefaultAsync(x => x.Id == id);

        if (entidadDB is null)
            return NotFound();

        var entidadDTO = mapper.Map<TDTO>(entidadDB);
        patchDocument.ApplyTo(entidadDTO, ModelState);

        var isValid = TryValidateModel(entidadDTO);
        if (!isValid)
            return BadRequest(ModelState);

        mapper.Map(entidadDTO, entidadDB);
        await context.SaveChangesAsync();

        return NoContent();
    }

    protected async Task<ActionResult> Delete<TEntidad>(int id) where TEntidad: class, IId, new() {
        var exist = await context.Set<TEntidad>().AnyAsync(x => x.Id == id);
        if(!exist) return NoContent();

        context.Remove(new TEntidad() { Id = id });
        await context.SaveChangesAsync();

        return NoContent();
    }
}