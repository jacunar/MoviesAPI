namespace MoviesAPI.Helpers; 
public static class Factorias {
    public static IFileStorage FileStorageService(IServiceProvider serviceProvider) {
        var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();

        if(env.IsDevelopment()) {
            var httpContextAccesor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            return new LocalFileStorage(env, httpContextAccesor);
        } else {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            return new AzureFileStorage(configuration);
        }
    }
}