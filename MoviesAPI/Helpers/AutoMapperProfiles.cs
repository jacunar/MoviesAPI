namespace MoviesAPI.Helpers; 
public class AutoMapperProfiles: Profile {
	public AutoMapperProfiles() {
		CreateMap<Genre, GenreDTO>().ReverseMap();
        CreateMap<GenreCreationDTO, Genre>();
        CreateMap<Actor, ActorDTO>().ReverseMap();
        CreateMap<ActorCreationDTO, Actor>();
    }
}