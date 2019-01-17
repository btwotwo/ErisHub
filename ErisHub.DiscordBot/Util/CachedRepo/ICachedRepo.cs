using System.Collections.Generic;
using System.Threading.Tasks;

namespace ErisHub.DiscordBot.Util.CachedRepo
{
    public interface ICachedRepo<T> where T : class
    {
        IReadOnlyCollection<T> Cache { get; }
        Task AddAsync(T item);
        Task UpdateAsync(T item);
        Task DeleteAsync(T item);
    }
}