using System.Threading.Tasks;

namespace ErisHub.DiscordBot.Util.CachedDbEntity
{
    public interface ICachedDbEntity<T> where T : class
    {
        T CachedValue { get; }
        Task UpdateAsync(T val);
    }
}