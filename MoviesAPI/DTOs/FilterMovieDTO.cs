namespace MoviesAPI.DTOs; 
public class FilterMovieDTO {
    public int Page { get; set; } = 1;
    public int RecordsPerPage { get; set; } = 10;
    public PaginationDTO Pagination { 
        get { return new PaginationDTO() { Page = Page, RecordsPerPage = RecordsPerPage }; }
    }

    public string Title { get; set; } = string.Empty;
    public int GenreId { get; set; }
    public bool OnCinemas { get; set; }
    public bool ProximosEstrenos { get; set; }

    public string FieldToOrder { get; set; } = string.Empty;
    public bool Ascending { get; set; }
}