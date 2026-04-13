using AkademiqRapidApi.Models;

namespace AkademiqRapidApi.Services.Interfaces
{
    public interface IMovieService
    {
        /// <summary>
        /// Retrieves a list of movies by category. Useful for library categories.
        /// Valid categories: "popular", "top_rated", "upcoming", "now_playing", "favorites", "trending"
        /// </summary>
        Task<List<MovieViewModel.Result>> GetMoviesAsync(string category = "popular");
    }
}
