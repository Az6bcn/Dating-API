using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dating.API.Model;
using Microsoft.EntityFrameworkCore;

namespace Dating.API.Data
{
    public class DataDbContext: DbContext
    {
        public DataDbContext(DbContextOptions<DataDbContext> options) : base(options) {}

        public DbSet<Value> Values { get; set; }

    }
}
