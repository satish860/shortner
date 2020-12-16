using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shortner.Core
{
    public class UrlRepository : IUrlRepository
    {
        private readonly IDatabase database;

        public UrlRepository(IDatabase database)
        {
            this.database = database;
        }

        public Task<bool> SaveUrl(long id, string url)
        {
            return this.database.StringSetAsync(id.ToString(), url);
        }

        public async Task<string> GetUrl(long id)
        {
            var value = await this.database.StringGetAsync(id.ToString());
            return value;
        }
    }
}
