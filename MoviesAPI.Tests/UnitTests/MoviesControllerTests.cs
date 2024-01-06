using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq;

namespace MoviesAPI.Tests.UnitTests;

[TestClass]
public class MoviesControllerTests : BasePruebas {
    private string CrearDataPrueba() {
        string nameDb = Guid.NewGuid().ToString();
        var context = ConstruirContext(nameDb);
        var genre = new Genre() { Name = "Genre 1" };
        var movies = new List<Movie>() {
            new Movie(){Title = "Pelicula 1", Premiere = new DateTime(2010,1,1), OnCimena = false},
            new Movie(){Title = "No Estrenada", Premiere = DateTime.Today.AddDays(1), OnCimena = false},
            new Movie(){Title = "Pelicula en cines", Premiere = DateTime.Today.AddDays(-1), OnCimena = true}
        };

        var peliculaConGenero = new Movie() {
            Title = "Pelicula con Genero",
            Premiere = new DateTime(2010,1,1),
            OnCimena = false
        };
        movies.Add(peliculaConGenero);

        context.Add(genre);
        context.AddRange(movies);
        context.SaveChanges();

        var peliculaGenero = new MoviesGenres() { GenreId = genre.Id, MovieId = peliculaConGenero.Id };
        context.Add(peliculaGenero);
        context.SaveChanges();

        return nameDb;
    }

    [TestMethod]
    public async Task FiltrarPorTitulo() {
        var nameDb = CrearDataPrueba();
        var mapper = ConfigurarAutoMapper();
        var context = ConstruirContext(nameDb);

        var controller = new MoviesController(context, mapper, null, null);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        var titulo = "Pelicula 1";
        var filtroDTO = new FilterMovieDTO() {
            Title = titulo,
            RecordsPerPage = 10
        };

        var respuesta = await controller.Filter(filtroDTO);
        var movies = respuesta.Value;
        Assert.AreEqual(1, movies.Count);
        Assert.AreEqual(titulo, movies[0].Title);
    }

    [TestMethod]
    public async Task FiltrarEnCines() {
        var nameDb = CrearDataPrueba();
        var mapper = ConfigurarAutoMapper();
        var context = ConstruirContext(nameDb);

        var controller = new MoviesController(context, mapper, null, null);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        var filtroDTO = new FilterMovieDTO() {
            OnCinemas = true
        };

        var respuesta = await controller.Filter(filtroDTO);
        var movies = respuesta.Value;
        Assert.AreEqual(1, movies.Count);
        Assert.AreEqual("Pelicula en cines", movies[0].Title);
    }

    [TestMethod]
    public async Task FiltrarProximosEstrenos() {
        var nameDb = CrearDataPrueba();
        var mapper = ConfigurarAutoMapper();
        var context = ConstruirContext(nameDb);

        var controller = new MoviesController(context, mapper, null, null);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        var filtroDTO = new FilterMovieDTO() {
            ProximosEstrenos = true
        };

        var respuesta = await controller.Filter(filtroDTO);
        var movies = respuesta.Value;
        Assert.AreEqual(1, movies.Count);
        Assert.AreEqual("No Estrenada", movies[0].Title);
    }

    [TestMethod]
    public async Task FiltrarPorGenero() {
        var nameDb = CrearDataPrueba();
        var mapper = ConfigurarAutoMapper();
        var context = ConstruirContext(nameDb);

        var controller = new MoviesController(context, mapper, null, null);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        int generoId = context.Genres.Select(x => x.Id).FirstOrDefault();
        var filtroDTO = new FilterMovieDTO() {
            GenreId = generoId
        };

        var respuesta = await controller.Filter(filtroDTO);
        var movies = respuesta.Value;
        Assert.AreEqual(1, movies.Count);
        Assert.AreEqual("Pelicula con Genero", movies[0].Title);
    }

    [TestMethod]
    public async Task FiltrarOrdenaTituloAscendente() {
        var nombreBD = CrearDataPrueba();
        var mapper = ConfigurarAutoMapper();
        var context = ConstruirContext(nombreBD);

        var controller = new MoviesController(context, mapper, null, null);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        var filtroDTO = new FilterMovieDTO() {
            FieldToOrder = "Title",
            Ascending = true
        };

        var respuesta = await controller.Filter(filtroDTO);
        var peliculas = respuesta.Value;

        var contexto2 = ConstruirContext(nombreBD);
        var peliculasDB = contexto2.Movies.OrderBy(x => x.Title).ToList();

        Assert.AreEqual(peliculasDB.Count, peliculas.Count);

        for (int i = 0; i < peliculasDB.Count; i++) {
            var peliculaDelControlador = peliculas[i];
            var peliculaDB = peliculasDB[i];

            Assert.AreEqual(peliculaDB.Id, peliculaDelControlador.Id);
        }
    }

    [TestMethod]
    public async Task FiltrarTituloDescendente() {
        var nombreBD = CrearDataPrueba();
        var mapper = ConfigurarAutoMapper();
        var contexto = ConstruirContext(nombreBD);

        var controller = new MoviesController(contexto, mapper, null, null);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        var filtroDTO = new FilterMovieDTO() {
            FieldToOrder = "Title",
            Ascending = false
        };

        var respuesta = await controller.Filter(filtroDTO);
        var peliculas = respuesta.Value;

        var contexto2 = ConstruirContext(nombreBD);
        var peliculasDB = contexto2.Movies.OrderByDescending(x =>x.Title).ToList();

        Assert.AreEqual(peliculasDB.Count, peliculas.Count);

        for (int i = 0; i < peliculasDB.Count; i++) {
            var peliculaDelControlador = peliculas[i];
            var peliculaDB = peliculasDB[i];

            Assert.AreEqual(peliculaDB.Id, peliculaDelControlador.Id);
        }
    }

    [TestMethod]
    public async Task FiltrarPorCampoIncorrectoDevuelvePeliculas() {
        var nombreBD = CrearDataPrueba();
        var mapper = ConfigurarAutoMapper();
        var contexto = ConstruirContext(nombreBD);

        var mock = new Mock<ILogger<MoviesController>>();

        var controller = new MoviesController(contexto, mapper, null, mock.Object);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        var filtroDTO = new FilterMovieDTO() {
            FieldToOrder = "abc",
            Ascending = true
        };

        var respuesta = await controller.Filter(filtroDTO);
        var peliculas = respuesta.Value;

        var context2 = ConstruirContext(nombreBD);
        var peliculasDB = context2.Movies.ToList();
        Assert.AreEqual(peliculasDB.Count, peliculas.Count);
        Assert.AreEqual(1, mock.Invocations.Count);
    }
}