using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace NoSqlWrapper.Data
{
    public class NoSQLDropCreateSeedAlways 
            : DropCreateDatabaseAlways<NoSQLContext>
    {
        protected override void Seed(NoSQLContext context)
        {
            base.Seed(context); // empty method call
            NoSQLSeedData.SeedDemoData(context);
        }
    }
}
