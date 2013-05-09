using NoSqlWrapper.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using NoSqlWrapper.Model;

namespace NoSqlWrapper
{
    public class Program
    {
        static void Main(string[] args)
        {
            // begging for configuring the initialization strategy ... eh
            string message = ConfigurationManager.AppSettings["howdy"];
            Console.WriteLine("from the configuration file: " + message);

            DbInitializationTools.DbInitializerStrategyFactory("CreateSeedAlways");
            using (var context = new NoSQLContext())
            {
                var count = context.Store.Count();
                Console.WriteLine("the number of items is: " + count.ToString());    
            }



            // Temp test of the system
            using (var context = new NoSQLContext())
            {
                Store<string> s = new Store<string>(context);

                var id1 = s.Create(@"P&P test 1");
                var id2 = s.Create(@"P&P test 2");

                var foo = s.Retrieve(id1);
                var bar = s.Retrieve(id2);

                Console.WriteLine(foo);
                Console.WriteLine(bar);

                context.SaveChanges();
            }

            Console.ReadKey();
        }
    }
}
