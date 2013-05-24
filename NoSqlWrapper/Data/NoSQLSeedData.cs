using NoSqlWrapper.Data.Entity;
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
            context.TypeVersion.Add(new TypeVersionEntity()
            {
                TypeName = typeof(String).AssemblyQualifiedName,
                TypeSignature = String.Empty,
                TypeVersionId = Guid.NewGuid()
            });
            context.Store.Add(new StoreEntity()
            {
                StoreId = Guid.NewGuid(),
                Value = "foo",
                TypeVersionId = context.TypeVersion.First().TypeVersionId
            });

            context.Store.Add(new StoreEntity()
            {
                StoreId = Guid.NewGuid(),
                Value = "bar",
                TypeVersionId = context.TypeVersion.First().TypeVersionId
            });
        }
    }
}
