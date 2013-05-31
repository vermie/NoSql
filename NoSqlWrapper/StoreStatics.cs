using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSqlWrapper
{

    public static class StoreUtility
    {

        public static Guid Create<T>(T instance)
        {
            using (var store = new Store())
            {
                var item= store.Create<T>(instance);
                store.SaveChanges();
                return item;
            }
        }
        public static void Update<T>(Guid id, T instance)
        {
            using (var store = new Store())
            {
                store.Update<T>(id, instance);
                store.SaveChanges();
            }
        }
        public static void Delete<T>(Guid id)
        {
            using (var store = new Store())
            {
                store.Delete<T>(id);
                store.SaveChanges();
            }
        }
        public static T TryRetrieve<T>(Guid id)
        {
            using (var store = new Store())
            {
                return store.TryRetrieve<T>(id);
            }
        }

    }

}
