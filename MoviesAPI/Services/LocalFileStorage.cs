
namespace MoviesAPI.Services;
public class LocalFileStorage : IFileStorage {
    private readonly IWebHostEnvironment env;
    private readonly IHttpContextAccessor contextAccessor;

    public LocalFileStorage(IWebHostEnvironment env, IHttpContextAccessor contextAccessor) {
        this.env = env;
        this.contextAccessor = contextAccessor;
    }

    public Task DeleteFile(string path, string container) {
        if (path != null) {
            var fileName = Path.GetFileName(path);
            string fileDirectory = Path.Combine(env.WebRootPath, container, fileName);

            if(File.Exists(fileDirectory)) 
                File.Delete(fileDirectory);
        }
        return Task.FromResult(0);
    }

    public async Task<string> EditFile(byte[] content, string extension, string container, string path, string contentType) {
        await DeleteFile(path, container);
        return await SaveFile(content, extension, container, contentType);
    }

    public async Task<string> SaveFile(byte[] content, string extension, string container, string contentType) {
        var fileName = $"{Guid.NewGuid()}{extension}";
        string folder = Path.Combine(env.WebRootPath, container);

        if(!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string path = Path.Combine(folder, fileName);
        await File.WriteAllBytesAsync(path, content);

        var urlActual = $"{contextAccessor.HttpContext.Request.Scheme}://{contextAccessor.HttpContext.Request.Host}";
        var urlBD = Path.Combine(urlActual, container, fileName).Replace("\\", "/");
        return urlBD;
    }
}