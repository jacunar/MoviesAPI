namespace MoviesAPI.Helpers; 
public static class HttpContextExtensions {
    public async static Task InsertParameterPagination<T>(this HttpContext httpContext,
        IQueryable<T> queryable, int recordsPerPage) {
        double quantity = await queryable.CountAsync();
        double quantityPages = Math.Ceiling(quantity / recordsPerPage);
        httpContext.Response.Headers.Add("cantidadPaginas", quantityPages.ToString());
    }
}