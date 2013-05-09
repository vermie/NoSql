using NoSqlWrapper.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace NoSqlWrapper.Data
{
    public class NoSQLSeedData
    {
        public static void SeedDemoData(NoSQLContext context)
        {
            
            context.Store.Add(new StoreEntity()
            {
                Id = Guid.NewGuid(),
                Value = "foo"
            });

            context.Store.Add(new StoreEntity()
            {
                Id = Guid.NewGuid(),
                Value = "bar"
            });
        }
    }
}
