using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Shortner.Core
{
    public class UniqueIdGenerator : IUniqueIdGenerator
    {
        private readonly IDatabase database;

        public UniqueIdGenerator(IDatabase database)
        {
            this.database = database;
        }
        public Task<long> GetNext()
        {
            return this.database.StringIncrementAsync("shortner");
        }


    }
}
