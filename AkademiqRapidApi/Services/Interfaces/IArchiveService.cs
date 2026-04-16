using AkademiqRapidApi.Models.Entities;

namespace AkademiqRapidApi.Services.Interfaces
{
    public interface IArchiveService
    {
        Task<List<ChatSession>> GetAllSessionsAsync();
        Task<ChatSession> GetSessionByIdAsync(int id);
        Task CreateSessionAsync(ChatSession session);
        Task AddMessageAsync(ChatMessage message);
    }
}
