using Microsoft.AspNetCore.Mvc;

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
}