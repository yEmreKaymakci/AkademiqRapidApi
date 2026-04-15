using System.Runtime.CompilerServices;

namespace AkademiqRapidApi.Services.Interfaces
{
    public interface IGeminiService
    {
        Task<string> GetChatResponseAsync(string userMessage);
        IAsyncEnumerable<string> GetChatResponseStreamAsync(string userMessage,
            [EnumeratorCancellation] CancellationToken cancellationToken = default);
    }
}
