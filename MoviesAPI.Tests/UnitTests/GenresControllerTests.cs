using MoviesAPI.Controllers;

namespace MoviesAPI.Tests.UnitTests;

[TestClass]
public class GenresControllerTests: BasePruebas {
    [TestMethod]
    public async Task ObtenerTodosLosGeneros() {
        // Preparar
        var nombreDb = Guid.NewGuid().ToString();
        var context = ConstruirContext(nombreDb);
        var mapper = ConfigurarAutoMapper();

        context.Genres.Add(new Genre { Name = "Genero 1" });
        context.Genres.Add(new Genre { Name = "Genero 2" });
        await context.SaveChangesAsync();

        var context2 = ConstruirContext(nombreDb);

        // Probar
        var controller = new GenresController(context2, mapper);
        var respuesta = await controller.Get();

        // Verificar
        var genres = respuesta.Value;
        Assert.AreEqual(2, genres.Count);
    }

    [TestMethod]
    public async Task ObtenerGeneroPorIdNoExistente() {
        // Preparar
        var nombreDb = Guid.NewGuid().ToString();
        var context = ConstruirContext(nombreDb);
        var mapper = ConfigurarAutoMapper();

        var controller = new GenresController(context, mapper);
        var respuesta = await controller.Get(1);

        var resultado = respuesta.Result as StatusCodeResult;
        Assert.AreEqual(404, resultado.StatusCode);
    }

    [TestMethod]
    public async Task ObtenerGeneroPorIdExistente() {
        // Preparar
        var nombreDb = Guid.NewGuid().ToString();
        var context = ConstruirContext(nombreDb);
        var mapper = ConfigurarAutoMapper();

        context.Genres.Add(new Genre { Name = "Genero 1" });
        context.Genres.Add(new Genre { Name = "Genero 2" });
        await context.SaveChangesAsync();

        var contexto2 = ConstruirContext(nombreDb);
        var controller = new GenresController(contexto2, mapper);

        int id = 1;
        var respuesta = await controller.Get(id);
        var result = respuesta.Value;
        Assert.AreEqual(id, result.Id);
    }

    [TestMethod]
    public async Task CrearGenero() {
        // Preparar
        var nombreDb = Guid.NewGuid().ToString();
        var context = ConstruirContext(nombreDb);
        var mapper = ConfigurarAutoMapper();

        var nuevoGenero = new GenreCreationDTO() { Name = "nuevo genero" };
        var controller = new GenresController(context, mapper);

        var respuesta = await controller.Post(nuevoGenero);
        var result = respuesta as CreatedAtRouteResult;
        Assert.IsNotNull(result);

        var contexto2 = ConstruirContext(nombreDb);
        var cantidad = await contexto2.Genres.CountAsync();
        Assert.AreEqual(1, cantidad);
    }

    [TestMethod]
    public async Task ActualizarGenero() {
        // Preparar
        var nombreDb = Guid.NewGuid().ToString();
        var context = ConstruirContext(nombreDb);
        var mapper = ConfigurarAutoMapper();

        context.Genres.Add(new Genre() { Name = "Genero 1" });
        await context.SaveChangesAsync();

        var contexto2 = ConstruirContext(nombreDb);
        var controller = new GenresController(contexto2, mapper);
        var generoCreationDTO = new GenreCreationDTO() { Name = "Nuevo nombre" };

        var respuesta = await controller.Put(1, generoCreationDTO);
        var result = respuesta as StatusCodeResult;
        Assert.AreEqual(204, result.StatusCode);

        var context3 = ConstruirContext(nombreDb);
        var existe = await context3.Genres.AnyAsync(x => x.Name == "Nuevo nombre");
        Assert.IsTrue(existe);
    }

    [TestMethod]
    public async Task IntentaBorrarGeneroNoExistente() {
        // Preparar
        var nombreBD = Guid.NewGuid().ToString();
        var contexto = ConstruirContext(nombreBD);
        var mapper = ConfigurarAutoMapper();

        var controller = new GenresController(contexto, mapper);

        var respuesta = await controller.Delete(1);
        var resultado = respuesta as StatusCodeResult;
        Assert.AreEqual(404, resultado.StatusCode);
    }

    [TestMethod]
    public async Task BorrarGeneroExitoso() {
        // Preparar
        var nombreDb = Guid.NewGuid().ToString();
        var context = ConstruirContext(nombreDb);
        var mapper = ConfigurarAutoMapper();

        context.Genres.Add(new Genre() { Name = "Genero 1" });
        await context.SaveChangesAsync();

        var contexto2 = ConstruirContext(nombreDb);
        var controller = new GenresController(contexto2, mapper);

        var respuesta = await controller.Delete(1);
        var result = respuesta as StatusCodeResult;
        Assert.AreEqual(204, result.StatusCode);

        var context3 = ConstruirContext(nombreDb);
        var exist = await context3.Genres.AnyAsync();
        Assert.IsFalse(exist);
    }
}