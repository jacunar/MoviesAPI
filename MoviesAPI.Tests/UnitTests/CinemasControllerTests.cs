using NetTopologySuite.Geometries;
using NetTopologySuite;

namespace MoviesAPI.Tests.UnitTests;

[TestClass]
public class CinemasControllerTests: BasePruebas {
    [TestMethod]
    public async Task ObtenerCinemasA5KmOMenos() {
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

        using (var context = LocalDbDatabaseInitializer.GetDbContextLocalDb(false)) {
            var salasDeCine = new List<Cinema>() {
                    new Cinema { Name = "Metrocentro",
                        Location = geometryFactory.CreatePoint(new Coordinate(-69.9388777, 18.4839233)) }
                };

            context.AddRange(salasDeCine);
            await context.SaveChangesAsync();
        }

        var filtro = new NearCinemaFilterDTO() {
            DistanceInKms = 5,
            Latitude = 18.481139,
            Longitude = -69.938950
        };

        using (var context = LocalDbDatabaseInitializer.GetDbContextLocalDb(false)) {
            var mapper = ConfigurarAutoMapper();
            var controller = new CinemasController(context, mapper, geometryFactory);
            var cinemas = await context.Cinemas.ToListAsync();
            var respuesta = await controller.Cercanos(filtro);
            var valor = respuesta.Value;
            Assert.AreEqual(3, valor.Count);
        }
    }
}