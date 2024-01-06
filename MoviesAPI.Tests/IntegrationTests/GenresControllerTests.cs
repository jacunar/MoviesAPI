using Newtonsoft.Json;

namespace MoviesAPI.Tests.IntegrationTests;

[TestClass]
public class GenresControllerTests: BasePruebas {
    private static readonly string url = "/api/genres";

    [TestMethod]
    public async Task ObtenerTodosLosGenerosListadoVacio() {
        var nameDB = Guid.NewGuid().ToString();
        var factory = ConstruirWebApplicationFactory(nameDB);

        var client = factory.CreateClient();
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var genres = JsonConvert.DeserializeObject<List<GenreDTO>>(await response.Content.ReadAsStringAsync());

        Assert.AreEqual(0, genres.Count());
    }

    [TestMethod]
    public async Task ObtenerTodosLosGeneros() {
        var nameDB = Guid.NewGuid().ToString();
        var factory = ConstruirWebApplicationFactory(nameDB);

        var context = ConstruirContext(nameDB);
        context.Genres.Add(new Genre() { Name = "Genero 1" });
        context.Genres.Add(new Genre() { Name = "Genero 2" });
        await context.SaveChangesAsync();

        var client = factory.CreateClient();
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var genres = JsonConvert.DeserializeObject<List<GenreDTO>>(await response.Content.ReadAsStringAsync());

        Assert.AreEqual(2, genres.Count());
    }

    [TestMethod]
    public async Task BorrarGenero() {
        var nameDB = Guid.NewGuid().ToString();
        var factory = ConstruirWebApplicationFactory(nameDB);

        var context = ConstruirContext(nameDB);
        context.Genres.Add(new Genre() { Name = "Genero 1" });
        await context.SaveChangesAsync();

        var client = factory.CreateClient();
        var response = await client.DeleteAsync($"{url}/1");
        response.EnsureSuccessStatusCode();

        var context2 = ConstruirContext(nameDB);
        var existe = await context2.Genres.AnyAsync();
        Assert.IsFalse(existe);
    }

    [TestMethod]
    public async Task BorrarGeneroRetorna401() {
        var nameDB = Guid.NewGuid().ToString();
        var factory = ConstruirWebApplicationFactory(nameDB, ignoreSecurity: false);

        var client = factory.CreateClient();
        var response = await client.DeleteAsync($"{url}/1");
        Assert.AreEqual("Unauthorized", response.ReasonPhrase);
    }
}