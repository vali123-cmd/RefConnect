using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RefConnect.Models;




using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RefConnect.DTOs.Chats;
using RefConnect.DTOs.Messages;
using RefConnect.DTOs.Shared;




namespace RefConnect.Services.Interfaces
{
    public interface IRefinePostTextAI
    {
        Task<string> RefineTextAsync(string inputText, CancellationToken ct = default);
        Task<bool> IsContentAppropriateAsync(string content, CancellationToken ct = default);
    }
}
