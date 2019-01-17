using System.Threading.Tasks;
using ErisHub.DiscordBot.Database;
using Microsoft.EntityFrameworkCore;

namespace ErisHub.DiscordBot.Util.CachedDbEntity
{
    public class CachedDbEntity<T> : ICachedDbEntity<T> where T: class
    {
        private readonly BotContext _db;

        public CachedDbEntity(BotContext db)
        {
            _db = db;
            UpdateCache().Wait();
        }

        public T CachedValue { get; private set; }

        public async Task UpdateAsync(T val)
        {
            _db.Update(val);
            await _db.SaveChangesAsync();
            await UpdateCache();
        }

        private async Task UpdateCache()
        {
            CachedValue = await _db.Set<T>().SingleOrDefaultAsync();
        }
    }
}
