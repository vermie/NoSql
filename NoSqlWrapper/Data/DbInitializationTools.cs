using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace NoSqlWrapper.Data
{
    public class DbInitializationTools
    {
        /// <summary> Decoupled way to initialize the database context.  This
        /// initializes the database for the current application domain
        /// </summary>
        /// <param name="initType">the initializer to use 
        ///     it currently supports  "CreateIfNew", "CreateIfChanged", "Always", "CreateSeedAlways", and "CreateSeedIfModified"
        /// </param>
        /// <returns>The selected initialization factory used or null if there is no db initializer</returns>
        public static void DbInitializerStrategyFactory(string initType)
        {
            switch (initType)
            {
                case "CreateIfNew":
                    Database.SetInitializer(new CreateDatabaseIfNotExists<NoSQLContext>());
                    break;

                case "CreateIfChanged":
                    Database.SetInitializer(new DropCreateDatabaseIfModelChanges<NoSQLContext>());
                    break;

                case "CreateAlways":
                    Database.SetInitializer(new DropCreateDatabaseAlways<NoSQLContext>());
                    break;

                case "CreateSeedAlways":
                    Database.SetInitializer(new NoSQLDropCreateSeedAlways());
                    break;

                case "CreateSeedIfModified":
                    Database.SetInitializer(new NoSQLDropCreateSeedIfModified());
                    break;

                default:
                    // no initializer strategy
                    break;
            }
        }
    }
}
