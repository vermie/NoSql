using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NoSqlWrapper.Data;
using NoSqlWrapper.Repositories;

namespace NoSqlWrapper.TestHarness
{
    public class Program
    {
        static void Main(string[] args)
        {
            DbInitializationTools.DbInitializerStrategyFactory("CreateIfChanged");

            //DoIt();
            //return;

            // begging for configuring the initialization strategy ... eh
            string message = ConfigurationManager.AppSettings["howdy"];
            Console.WriteLine("from the configuration file: " + message);
       
            using (var context = new NoSQLContext())
            {
                var count = context.Store.Count();
                Console.WriteLine("the number of items is: " + count.ToString());
            }

            using (var context = new NoSQLContext())
            {
                context.Database.ExecuteSqlCommand("Delete from Store");
                context.Database.ExecuteSqlCommand("Delete from typeversion");
            }



            // Temp test of the system
            Guid id1, id2;
            using (var context = new NoSQLContext())
            {
                NoSqlRepository<string> s = new NoSqlRepository<string>(context);

                id1 = s.Create(@"P&P test 1");
                id2 = s.Create(@"P&P test 2");

                context.SaveChanges();
            }

            using (var context = new NoSQLContext())
            {
                NoSqlRepository<string> s = new NoSqlRepository<string>(context);

                var foo = s.Retrieve(id1);
                var bar = s.Retrieve(id2);

                Console.WriteLine(foo);
                Console.WriteLine(bar);
            }

            using (var store = new Store())
            {
                var one = BuildMeOne();

                store.Create(one);

                store.SaveChanges();
            }

            using (var store = new Store())
            {
                var readBack = store.TryRetrieve<SimplePoco>(new Guid("DA722261-062C-43DF-B64C-B17829A58440"));

                var yo = 3334;
            }

            Console.ReadKey();
        }

        static void DoIt()
        {
            NoSqlWrapper.Serialization.JsonSerializer<SimplePoco> serial = 
                new Serialization.JsonSerializer<SimplePoco>();

            var settings = new JsonSerializerSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //settings.Error = new EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs>(JsonErrors);
            //settings.MissingMemberHandling = MissingMemberHandling.Error;
            settings.TypeNameHandling = TypeNameHandling.All;

            serial.DefaultJsonSettings = settings;
            serial.DefaultJsonSettings.TypeNameHandling = TypeNameHandling.All;

            var one = BuildMeOne();
            var oneJson = serial.Serialize(one);

            var somejson = "{'SimplePocoId':1888,'MadeUp':333,'RootName':'Rooty','Children':[{'ChildName':'Childey'}]}";
            var someJsonResult = serial.Deserialize(somejson);

            var someMoreJson = "{'$type':'NoSqlWrapper.TestHarness.SimplePoco, NoSqlWrapper.TestHarness.Console','SimplePocoId':1888,'RootName':'Rooty','Children':{'$type':'NoSqlWrapper.TestHarness.SimplePocoChildList, NoSqlWrapper.TestHarness.Console','$values':[{'$type':'NoSqlWrapper.TestHarness.SimplePocoChild, NoSqlWrapper.TestHarness.Console','ChildName':'Childey','Something':{'$type':'NoSqlWrapper.TestHarness.BSomething, NoSqlWrapper.TestHarness.Console','BSome':'9c10755d-7caf-4fab-a716-754fe88bacb9','Amount':333.0}}]}}";
            var someMoreJsonResult = serial.Deserialize(someMoreJson);

            var dd = 23;

            Console.ReadKey();
        }
        static void JsonErrors(Object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {


            var dd = 344;
        }

        static SimplePoco BuildMeOne()
        {
            SimplePoco poco = new SimplePoco()
            {
                RootName = "Rooty",
                SimplePocoId = 1888
            };
            poco.Children.Add(new SimplePocoChild() { ChildName = "Childey", Parent = poco });

            poco.Children[0].Something = new BSomething() { Amount = 333M, BSome = Guid.NewGuid() };

            return poco;
        }
    }

    public class SimplePoco
    {
        public Int32 SimplePocoId
        {
            get;
            set;
        }
        public String RootName
        {
            get;
            set;
        }
        private DateTime Yo
        {
            get;
            set;
        }

        public SimplePocoChildList Children
        {
            get;
            set;
        }

        public SimplePoco()
        {
            this.Children = new SimplePocoChildList();
            Yo = DateTime.Now;
        }
    }
    public class SimplePocoChildList : List<SimplePocoChild>
    {

    }
    public class SimplePocoChild
    {
        public SimplePoco Parent
        {
            get;
            set;
        }
        public String ChildName
        {
            get;
            set;
        }
        public ISomething Something
        {
            get;
            set;
        }
    }

    public class ASomething : ISomething
    {
        public String ASome
        {
            get;
            set;
        }
        public decimal Amount
        {
            get;
            set;
        }
    }
    public class BSomething : ISomething
    {
        public Guid BSome
        {
            get;
            set;
        }
        public decimal Amount
        {
            get;
            set;
        }
    }
    public interface ISomething
    {
         Decimal Amount{get;set;}
    }


    class MyJsonConverter : Newtonsoft.Json.JsonConverter
    {
        private static readonly string ISomething = typeof(ISomething).FullName;
        //private static readonly string IENTITY_FULLNAME = typeof(Interfaces.IEntity).FullName;


        public override bool CanConvert(Type objectType)
        {
            if (objectType.FullName == ISomething)
                //|| objectType.FullName == IENTITY_FULLNAME)
            {
                return true;
            }
            return false;
        }

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (objectType.FullName == ISomething)
                return serializer.Deserialize(reader, typeof(ASomething));
            //else if (objectType.FullName == IENTITY_FULLNAME)
            //    return serializer.Deserialize(reader, typeof(DTO.ClientEntity));

            throw new NotSupportedException(string.Format("Type {0} unexpected.", objectType));
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

    }
}
