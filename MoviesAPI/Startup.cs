using Microsoft.OpenApi.Models;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace MoviesAPI;
public class Startup {
    public Startup(IConfiguration configuration) {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
        //services.AddTransient<IFileStorage, AzureFileStorage>();
        services.AddTransient<IFileStorage, LocalFileStorage>();
        services.AddHttpContextAccessor();

        services.AddAutoMapper(typeof(Startup));
        services.AddSingleton<GeometryFactory>(NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326));
        services.AddSingleton(prov => {
            new MapperConfiguration(conf => {
                var geometryFactory = prov.GetRequiredService<GeometryFactory>();
                conf.AddProfile(new AutoMapperProfiles(geometryFactory));
            }).CreateMapper()}
        );
        
        services.AddControllers().AddNewtonsoftJson();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(s => {
            s.SwaggerDoc("v1", new OpenApiInfo { Title = "Movies Web API", Version = "v1" });
        });

        services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
            opt => opt.UseNetTopologySuite()));
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
        if (env.IsDevelopment()) {
            app.UseDeveloperExceptionPage();
        }

        app.UseSwagger();
        app.UseSwaggerUI(c => { 
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Movies Web API v1");
            c.RoutePrefix = "swagger";
        });
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => {
            endpoints.MapControllers();
        });
    }
}