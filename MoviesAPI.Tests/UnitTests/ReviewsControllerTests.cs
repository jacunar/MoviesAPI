using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace MoviesAPI.Tests.UnitTests;

[TestClass]
public class ReviewsControllerTests: BasePruebas {
    [TestMethod]
    public async Task UsuarioNoPuedeCrearDosReviewsParaLaMismaPelicula() {
        var nameDb = Guid.NewGuid().ToString();
        var context = ConstruirContext(nameDb);
        CrearPeliculas(nameDb);

        var peliculaId = context.Movies.Select(x => x.Id).FirstOrDefault();
        var review1 = new Review() {
            MovieId = peliculaId,
            UsuarioId = usuarioPorDefectoId,
            Puntuacion = 5
        };

        context.Add(review1);
        await context.SaveChangesAsync();

        var context2 = ConstruirContext(nameDb);
        var mapper = ConfigurarAutoMapper();

        var controller = new ReviewController(context2, mapper);
        controller.ControllerContext = ConstruirControllerContext();

        var reviewCreationDTO = new ReviewCreationDTO() { Puntuacion = 5 };
        var respuesta = await controller.Post(peliculaId, reviewCreationDTO);

        var valor = respuesta as IStatusCodeActionResult;
        Assert.AreEqual(400, valor.StatusCode.Value);
    }

    [TestMethod]
    public async Task CrearReview() {
        var nameDb = Guid.NewGuid().ToString();
        var context = ConstruirContext(nameDb);
        CrearPeliculas(nameDb);

        var peliculaId = context.Movies.Select(x => x.Id).FirstOrDefault();

        var context2 = ConstruirContext(nameDb);
        var mapper = ConfigurarAutoMapper();
        var controller = new ReviewController(context2, mapper);
        controller.ControllerContext = ConstruirControllerContext();

        var reviewCreationDTO = new ReviewCreationDTO() { Puntuacion = 5 };
        var respuesta = await controller.Post(peliculaId, reviewCreationDTO);
        var valor = respuesta as NoContentResult;
        Assert.IsNotNull(valor);

        var context3 = ConstruirContext(nameDb);
        var reviewDb = context3.Reviews.First();
        Assert.AreEqual(usuarioPorDefectoId, reviewDb.UsuarioId);
    }

    private void CrearPeliculas(string nombreDb) {
        var context = ConstruirContext(nombreDb);
        context.Movies.Add(new Movie() { Title = "pelicula 1" });
        context.SaveChanges();
    }
}