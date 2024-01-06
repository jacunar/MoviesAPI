using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using NetTopologySuite;

namespace MoviesAPI;
public class ApplicationDbContext : IdentityDbContext {
    public ApplicationDbContext(DbContextOptions options) : base(options) {
    }

    public DbSet<Genre> Genres { get; set; }
    public DbSet<Actor> Actors { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<MoviesActors> MoviesActors { get; set; }
    public DbSet<MoviesGenres> MoviesGenres { get; set; }
    public DbSet<Cinema> Cinemas { get; set; }
    public DbSet<MoviesCinemas> MoviesCinemas { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<MoviesActors>().HasKey(x => new { x.ActorId, x.MovieId });
        modelBuilder.Entity<MoviesGenres>().HasKey(x => new { x.MovieId, x.GenreId });
        modelBuilder.Entity<MoviesCinemas>().HasKey(x => new { x.MovieId, x.CinemaId });

        SeedData(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder) {
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

        modelBuilder.Entity<Cinema>()
           .HasData(new List<Cinema> {
                new Cinema {Id = 1, Name = "Agora", Location = geometryFactory.CreatePoint(new Coordinate(-69.9388777, 18.4839233))},
                new Cinema {Id = 4, Name = "Sambil", Location = geometryFactory.CreatePoint(new Coordinate(-69.9118804, 18.4826214))},
                new Cinema {Id = 5, Name = "Megacentro", Location = geometryFactory.CreatePoint(new Coordinate(-69.856427, 18.506934))},
                new Cinema {Id = 6, Name = "Village East Cinema", Location = geometryFactory.CreatePoint(new Coordinate(-73.986227, 40.730898))}
           });

        //var rolAdminId = "9aae0b6d-d50c-4d0a-9b90-2a6873e3845d";
        //var usuarioAdminId = "5673b8cf-12de-44f6-92ad-fae4a77932ad";

        //var rolAdmin = new IdentityRole() {
        //    Id = rolAdminId,
        //    Name = "Admin",
        //    NormalizedName = "Admin"
        //};

        //var passwordHasher = new PasswordHasher<IdentityUser>();

        //var username = "felipe@hotmail.com";

        //var usuarioAdmin = new IdentityUser() {
        //    Id = usuarioAdminId,
        //    UserName = username,
        //    NormalizedUserName = username,
        //    Email = username,
        //    NormalizedEmail = username,
        //    PasswordHash = passwordHasher.HashPassword(null, "Aa123456!")
        //};

        //modelBuilder.Entity<IdentityUser>()
        //    .HasData(usuarioAdmin);

        //modelBuilder.Entity<IdentityRole>()
        //    .HasData(rolAdmin);

        //modelBuilder.Entity<IdentityUserClaim<string>>()
        //    .HasData(new IdentityUserClaim<string>() {
        //        Id = 1,
        //        ClaimType = ClaimTypes.Role,
        //        UserId = usuarioAdminId,
        //        ClaimValue = "Admin"
        //    });
    }
}