namespace MoviesAPI.Helpers; 
public static class QueryableExtensions {
    public static IQueryable<T> Page<T>(this IQueryable<T> queryable, PaginationDTO paginationDTO) {
        return queryable.Skip((paginationDTO.Page - 1) * paginationDTO.RecordsPerPage)
            .Take(paginationDTO.RecordsPerPage);
    }
}