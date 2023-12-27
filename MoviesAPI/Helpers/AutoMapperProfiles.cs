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
        if(movieCreationDTO.Actors is null) 
            return resultado;

        foreach (var actor in movieCreationDTO.Actors) {
            resultado.Add(new MoviesActors() { ActorId = actor.ActorId, Character = actor.Character });
        }

        return resultado;
    }
}