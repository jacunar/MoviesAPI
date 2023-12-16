using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using System.Text;

namespace MoviesAPI;
public class Startup {
    public Startup(IConfiguration configuration) {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(s => {
            s.SwaggerDoc("v1", new OpenApiInfo { Title = "Movies Web API", Version = "v1" });
        });
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