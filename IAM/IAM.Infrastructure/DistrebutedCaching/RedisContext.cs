using IAM.Application.Common.Repositories;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDatabase = StackExchange.Redis.IDatabase;

namespace IAM.Infrastructure.DistrebutedCaching
{
    using StackExchange.Redis;
using Microsoft.Extensions.Configuration;

namespace IAM.Infrastructure.DistrebutedCaching
{
    public class RedisContext : ICachingContext
    {
        private readonly IDatabase _db;

        public RedisContext(IConnectionMultiplexer muxer)
        {
            _db = muxer.GetDatabase();
        }

        public string Get(string email) => _db.StringGet(email);

        public void Set(string email, string code)
        {
            var ttl = TimeSpan.FromMinutes(2);
            _db.StringSet(email, code, ttl);
        }
    }
}

}
