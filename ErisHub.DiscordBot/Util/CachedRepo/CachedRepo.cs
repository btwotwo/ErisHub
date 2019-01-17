using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErisHub.DiscordBot.Database;
using Microsoft.EntityFrameworkCore;

namespace ErisHub.DiscordBot.Util.CachedRepo
{

    public class CachedRepo<T> : ICachedRepo<T> where T : class
    {
        private readonly BotContext _db;
        public IReadOnlyCollection<T> Cache { get; private set; }

        public CachedRepo(BotContext db)
        {
            _db = db;
            UpdateCache().Wait();
        }

        public async Task AddAsync(T item)
        {
            await _db.AddAsync(item);
            await _db.SaveChangesAsync();

            await UpdateCache();
        }


        public async Task UpdateAsync(T item)
        {
            _db.Update(item);
            await _db.SaveChangesAsync();

            await UpdateCache();
        }

        public async Task DeleteAsync(T item)
        {
            _db.Remove(item);
            await _db.SaveChangesAsync();

            await UpdateCache();
        }

        private async Task UpdateCache()
        {
            Cache = await _db.Set<T>().ToListAsync();
        }
    }
}
