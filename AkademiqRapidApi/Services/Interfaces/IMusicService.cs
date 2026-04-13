using AkademiqRapidApi.Models;

namespace AkademiqRapidApi.Services.Interfaces
{
    public interface IMusicService
    {
        /// <summary>
        /// Gets the top tracks from Deezer Chart API.
        /// </summary>
        Task<List<MusicViewModel.Track>> GetTopTracksAsync();
        
        /// <summary>
        /// Gets tracks for a specific genre or playlist if needed. (Optional implementation)
        /// </summary>
        Task<List<MusicViewModel.Track>> GetPlaylistTracksAsync(long playlistId);
    }
}
