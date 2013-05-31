using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NoSqlWrapper.Data;
using NoSqlWrapper.Migration;
using NoSqlWrapper.Repositories;

namespace NoSqlWrapper.TestHarness
{

    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                DbInitializationTools.DbInitializerStrategyFactory("CreateIfChanged");

                Console.WriteLine("Clear database?");
                var instruction = Console.ReadKey();
                if (instruction.Key == ConsoleKey.Y)
                {
                    using (var context = new NoSQLContext())
                    {
                        context.Database.ExecuteSqlCommand("Delete from Store");
                        context.Database.ExecuteSqlCommand("Delete from typeversion");
                        context.Database.ExecuteSqlCommand("Delete from StoreArchive");
                    }
                }

                //DoIt();
                MigrationsFuck();
                return;

                // Temp test of the system
                Guid id1, id2;
                using (var context = new NoSQLContext())
                {
                    NoSqlRepository s = new NoSqlRepository(context);

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
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }

            Console.ReadKey();
        }

        static void DoIt()
        {
            Store storey = new Store();

            var thingy = BuildMeOne();

            var id = storey.Create(thingy);
            storey.SaveChanges();

            var rebuilt = storey.TryRetrieve<SimplePoco>(id);

            rebuilt.RootName = "Update now!!!";
            storey.Update(id, rebuilt);
            storey.SaveChanges();

            //storey.Delete<SimplePoco>(id);
            //storey.SaveChanges();
            var firstEntryId = (new NoSQLContext()).Store.OrderBy(a => a.DateCreated).First().StoreId;
            var firstItem = StoreUtility.TryRetrieve<SimplePoco>(firstEntryId);
            firstItem.RootName += " and reborn!!!";
            StoreUtility.Update(firstEntryId, firstItem);

            Console.ReadKey();
        }
        static void MigrationsFuck()
        {
            Migrations miggy = Migrations.Instance;

            //the simple case
            miggy.RegisterMigration(new TestMigration()
            {
                SourceTypeVersionId = new Guid("00000000-5B32-4980-9B09-F6A28A52BF86"),
                TargetTypeVersionId = new Guid("90000000-5B32-4980-9B09-F6A28A52BF86")
            });
            //add an 'infinite loop' to migrations...
            miggy.RegisterMigration(new TestMigration()
            {
                SourceTypeVersionId = new Guid("10000000-5B32-4980-9B09-F6A28A52BF86"),
                TargetTypeVersionId = new Guid("00000000-5B32-4980-9B09-F6A28A52BF86")
            });
            miggy.RegisterMigration(new TestMigration()
            {
                SourceTypeVersionId = new Guid("00000000-5B32-4980-9B09-F6A28A52BF86"),
                TargetTypeVersionId = new Guid("10000000-5B32-4980-9B09-F6A28A52BF86")
            });
            miggy.RegisterMigration(new TestMigration()
            {
                SourceTypeVersionId = new Guid("10000000-5B32-4980-9B09-F6A28A52BF86"),
                TargetTypeVersionId = new Guid("20000000-5B32-4980-9B09-F6A28A52BF86")
            });
            miggy.RegisterMigration(new TestMigration()
            {
                SourceTypeVersionId = new Guid("20000000-5B32-4980-9B09-F6A28A52BF86"),
                TargetTypeVersionId = new Guid("30000000-5B32-4980-9B09-F6A28A52BF86")
            });
            miggy.RegisterMigration(new TestMigration()
            {
                SourceTypeVersionId = new Guid("30000000-5B32-4980-9B09-F6A28A52BF86"),
                TargetTypeVersionId = new Guid("40000000-5B32-4980-9B09-F6A28A52BF86")
            });
            

            var noPath = miggy.FindMigrationPath(
                new Guid("11000000-5B32-4980-9B09-F6A28A52BF86"),
                new Guid("30000000-5B32-4980-9B09-F6A28A52BF86"));

            var simplePath = miggy.FindMigrationPath(
                new Guid("00000000-5B32-4980-9B09-F6A28A52BF86"),
                new Guid("30000000-5B32-4980-9B09-F6A28A52BF86"));

            

            var noInfiniteLoopPath = miggy.FindMigrationPath(
                new Guid("00000000-5B32-4980-9B09-F6A28A52BF86"),
                new Guid("30000000-5B32-4980-9B09-F6A28A52BF86"));
        }
        static void JsonExperiments()
        {
            NoSqlWrapper.Serialization.JsonSerializer<SimplePoco> serial = 
                new Serialization.JsonSerializer<SimplePoco>();

            var settings = new JsonSerializerSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //settings.Error = new EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs>(JsonErrors);
            settings.MissingMemberHandling = MissingMemberHandling.Error;
            settings.TypeNameHandling = TypeNameHandling.All;

            //serial.JsonSettings = new JsonSerializerSettings();
            serial.Options = new Conventions.Options();

            var one = BuildMeOne();
            var oneJson = serial.Serialize(one);

            var somejson = "{'SimplePocoId':1888,'RootName':'Rooty','Children':[{'ChildName':'Childey'}]}";
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

    //[NoSqlWrapper.Conventions.TypeSignature("{45A218D0-4BFC-43BA-ACCB-E6AB59BA2C6B}", "YOOO!")]
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
        public String NewFieldChange
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
            //this.NewField = "newey";
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
