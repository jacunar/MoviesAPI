using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MoviesAPI.Helpers;
public class ExistMovieAttribute : Attribute, IAsyncResultFilter {
    private readonly ApplicationDbContext Dbcontext;

    public ExistMovieAttribute(ApplicationDbContext context) {
        this.Dbcontext = context;
    }
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next) {
        var peliculaIdObject = context.HttpContext.Request.RouteValues["movieId"];

        if (peliculaIdObject == null)
            return;

        var peliculaId = int.Parse(peliculaIdObject.ToString());
        var existePelicula = await Dbcontext.Movies.AnyAsync(x => x.Id == peliculaId);

        if (!existePelicula) {
            context.Result = new NotFoundResult();
        } else 
            await next();
    }
}