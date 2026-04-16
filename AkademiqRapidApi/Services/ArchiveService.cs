using AkademiqRapidApi.Models.Contexts;
using AkademiqRapidApi.Models.Entities;
using AkademiqRapidApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AkademiqRapidApi.Services
{
    public class ArchiveService : IArchiveService
    {
        private readonly AkademiqDbContext _context;

        public ArchiveService(AkademiqDbContext context)
        {
            _context = context;
        }

        public async Task AddMessageAsync(ChatMessage message)
        {
            await _context.ChatMessages.AddAsync(message);
            await _context.SaveChangesAsync();
        }

        public async Task CreateSessionAsync(ChatSession session)
        {
            await _context.ChatSessions.AddAsync(session);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ChatSession>> GetAllSessionsAsync()
        {
            return await _context.ChatSessions.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        public async Task<ChatSession> GetSessionByIdAsync(int id)
        {
            return await _context.ChatSessions.Include(s => s.Messages).FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
