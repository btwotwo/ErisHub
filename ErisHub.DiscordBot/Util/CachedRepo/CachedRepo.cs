using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErisHub.DiscordBot.Database;
using ErisHub.DiscordBot.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace ErisHub.DiscordBot.Util.CachedRepo
{

    public class CachedRepo<T> : ICachedRepo<T> where T : class, IDbModel
    {
        private readonly BotContext _db;
        public IReadOnlyCollection<T> Cache { get; private set; }

        public CachedRepo(BotContext db)
        {
            _db = db;
            Cache = _db.Set<T>().ToList();
        }

        public async Task AddAsync(T item)
        {
            await _db.AddAsync(item);
            await _db.SaveChangesAsync();

            await UpdateCacheAsync();
        }


        public async Task UpdateAsync(T item)
        {
            _db.Update(item);
            await _db.SaveChangesAsync();

            await UpdateCacheAsync();
        }

        public async Task DeleteAsync(T item)
        {
            _db.Remove(item);
            await _db.SaveChangesAsync();

            await UpdateCacheAsync();
        }

        private async Task UpdateCacheAsync()
        {
            Cache = await _db.Set<T>().ToListAsync();
        }
    }
}
