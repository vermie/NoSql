using NoSqlWrapper.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace NoSqlWrapper.Data
{
    public class NoSQLContext : DbContext
    {
        public DbSet<StoreEntity> Store { get; set; }
        
    }
}
