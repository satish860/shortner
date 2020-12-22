using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Core.Implementations;

namespace Shortner.Api
{
    public static class RedisConnection
    {
        private static RedisCacheConnectionPoolManager ConnectionPoolManager { get; set; }

        public static IDatabase GetDatabase()
        {
            if(ConnectionPoolManager==null)
            {
                throw new Exception("Connection is not initialized. Use RedisConnection.Connect to initialize the connection");
            }
            return ConnectionPoolManager.GetConnection().GetDatabase();
        }

        public static void Connect(string redisIp, int port)
        {
            if (ConnectionPoolManager != null)
            {
                return;
            }

            ConnectionPoolManager = new RedisCacheConnectionPoolManager(new RedisConfiguration {
                Hosts = new RedisHost[] {
                    new RedisHost {
                        Host = redisIp,
                        Port=port
                    }
                }
            });
        }
    }
}
