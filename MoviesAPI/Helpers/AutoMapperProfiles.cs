using Microsoft.IdentityModel.Tokens;

namespace MoviesAPI.Helpers; 
public class AutoMapperProfiles: Profile {
	public AutoMapperProfiles() {
		CreateMap<Genre, GenreDTO>().ReverseMap();
        CreateMap<GenreCreationDTO, Genre>();

        CreateMap<Actor, ActorDTO>().ReverseMap();
        CreateMap<ActorCreationDTO, Actor>().
            ForMember(x => x.Picture, o => o.Ignore());
        CreateMap<ActorPatchDTO, Actor>().ReverseMap();

        CreateMap<Movie,  MovieDTO>().ReverseMap();
        CreateMap<MovieCreationDTO, Movie>()
                .ForMember(m => m.Poster, opt => opt.Ignore())
                .ForMember(x => x.MoviesGenres, opt => opt.MapFrom(MapMoviesGenres))
                .ForMember(x => x.MoviesActors, opt => opt.MapFrom(MapMoviesActors));

        CreateMap<MoviePatchDTO, Movie>().ReverseMap();

        CreateMap<Movie, MovieDetailDTO>()
                .ForMember(x => x.Genres, opt => opt.MapFrom(MapMoviesGenres))
                .ForMember(x => x.Actors, opt => opt.MapFrom(MapMoviesActors));
    }

    private List<ActorMovieDetailDTO> MapMoviesActors(Movie movie, MovieDetailDTO movieDetailDTO) {
        var result = new List<ActorMovieDetailDTO>();

        if (movie.MoviesActors is null)
            return result;

        foreach (var item in movie.MoviesActors) {
            result.Add(new ActorMovieDetailDTO {
                ActorId = item.ActorId,
                Character = item.Character,
                ActorName = item.Actor.Name
            });
        }

        return result;
    }

    private List<GenreDTO> MapMoviesGenres(Movie movie, MovieDetailDTO movieDetailDTO) {
        var result = new List<GenreDTO>();
        if (movie.MoviesGenres is null)
            return result;

        foreach (var item in movie.MoviesGenres) {
            result.Add(new GenreDTO() { Id = item.GenreId, Name = item.Genre.Name });
        }

        return result;
    }

    private List<MoviesGenres> MapMoviesGenres(MovieCreationDTO movieCreationDTO, Movie movie) {
        var resultado = new List<MoviesGenres>();
        if (movieCreationDTO.GenresIDs is null)
            return resultado;

        foreach (var id in movieCreationDTO.GenresIDs) {
            resultado.Add(new MoviesGenres() { GenreId = id });
        }

        return resultado;
    }

    private List<MoviesActors> MapMoviesActors(MovieCreationDTO movieCreationDTO, Movie movie) {
        var resultado = new List<MoviesActors>();
        if (movieCreationDTO.Actors is null)
            return resultado;

        foreach (var actor in movieCreationDTO.Actors) {
            resultado.Add(new MoviesActors() { ActorId = actor.ActorId, Character = actor.Character });
        }

        return resultado;
    }
}