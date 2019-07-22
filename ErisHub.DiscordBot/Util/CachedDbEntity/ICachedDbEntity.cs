using System.Threading.Tasks;
using ErisHub.DiscordBot.Database.Models;

namespace ErisHub.DiscordBot.Util.CachedDbEntity
{
    public interface ICachedDbEntity<T> where T : class, IDbModel
    {
        T CachedValue { get; }
        Task UpdateAsync(T val);
    }
}