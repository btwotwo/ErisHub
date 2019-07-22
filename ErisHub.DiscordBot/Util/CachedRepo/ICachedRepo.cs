using System.Collections.Generic;
using System.Threading.Tasks;
using ErisHub.DiscordBot.Database.Models;

namespace ErisHub.DiscordBot.Util.CachedRepo
{
    public interface ICachedRepo<T> where T : class, IDbModel
    {
        IReadOnlyCollection<T> Cache { get; }
        Task AddAsync(T item);
        Task UpdateAsync(T item);
        Task DeleteAsync(T item);
    }
}