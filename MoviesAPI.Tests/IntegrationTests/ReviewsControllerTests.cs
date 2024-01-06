using Azure;
using Newtonsoft.Json;

namespace MoviesAPI.Tests.IntegrationTests;

[TestClass]
public class ReviewsControllerTests: BasePruebas {
    private static readonly string url = "/api/movies/1/reviews";

    [TestMethod]
    public async Task ObtenerReviewDevuelve404PeliculaNoExiste() {
        var nameDB = Guid.NewGuid().ToString();
        var factory = ConstruirWebApplicationFactory(nameDB);

        var client = factory.CreateClient();
        var respuesta = await client.GetAsync(url);
        Assert.AreEqual(200, (int)respuesta.StatusCode);
        var reviews = await respuesta.Content.ReadAsStringAsync();
        Assert.AreEqual("", reviews);
    }

    [TestMethod]
    public async Task ObtenerReviewsDevuelveListadoVacio() {
        var nameDB = Guid.NewGuid().ToString();
        var factory = ConstruirWebApplicationFactory(nameDB);

        var context = ConstruirContext(nameDB);
        context.Movies.Add(new Movie() { Title = "Pelicula 1" });
        await context.SaveChangesAsync();

        var client = factory.CreateClient();
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var reviews = JsonConvert.DeserializeObject<List<ReviewDTO>>(await response.Content.ReadAsStringAsync());
        Assert.AreEqual(0, reviews.Count);
    }
}