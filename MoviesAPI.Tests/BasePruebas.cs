using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using MoviesAPI.Helpers;
using NetTopologySuite;
using Newtonsoft.Json.Serialization;
using System.Security.Claims;

namespace MoviesAPI.Tests;
public class BasePruebas {
    protected string usuarioPorDefectoId = "9722b56a-77ea-4e41-941d-e319b6eb3712";
    protected string usuarioPorDefectoEmail = "ejemplo@hotmail.com";

    protected ApplicationDbContext ConstruirContext(string nombreDb) {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(nombreDb).Options;

        return new ApplicationDbContext(options);
    }

    protected IMapper ConfigurarAutoMapper() {
        var config = new MapperConfiguration(options => {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            options.AddProfile(new AutoMapperProfiles(geometryFactory));
        });

        return config.CreateMapper();
    }

    protected ControllerContext ConstruirControllerContext() {
        var usuario = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
            new Claim(ClaimTypes.Name, usuarioPorDefectoEmail),
            new Claim(ClaimTypes.Email, usuarioPorDefectoEmail),
            new Claim(ClaimTypes.NameIdentifier, usuarioPorDefectoId)
        }));

        return new ControllerContext() {
            HttpContext = new DefaultHttpContext() { User = usuario }
        };
    }

    protected WebApplicationFactory<Startup> ConstruirWebApplicationFactory(string nameDb, bool ignoreSecurity = true) {
        var factory = new WebApplicationFactory<Startup>();
        factory = factory.WithWebHostBuilder(builder => {
            builder.ConfigureTestServices(services => {
                var dBContextDescriptor = services.SingleOrDefault(d =>
                        d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (dBContextDescriptor != null)
                    services.Remove(dBContextDescriptor);

                services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase(nameDb));

                if (ignoreSecurity) {
                    services.AddSingleton<IAuthorizationHandler, AllowAnonymousHandler>();
                    services.AddControllers(options => {
                        options.Filters.Add(new UsuarioFalsoFiltro());
                    });
                }
            });
        });

        return factory;
    }
}