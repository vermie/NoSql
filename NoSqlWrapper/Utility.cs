using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NoSqlWrapper.Conventions;
using NoSqlWrapper.Serialization;
using System.Reflection;

namespace NoSqlWrapper
{
    internal static class Utility
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }
        public static void TryDispose(this IDisposable disposable)
        {
            if (disposable != null)
                disposable.Dispose();
        }

        public static ISerializer<T> Get<T>(this ISerializerFactory serializer, Options options)
        {
            var serialy = serializer.Get<T>();
            serialy.Options = options;
            return serialy;
        }


        public static T TryGetCustomAttribute<T>(this Type type, Boolean inherit = false)
            where T : Attribute
        {
            return type.GetCustomAttribute<T>(inherit) as T;
        }

        public static Migration.MigrationKey MigrationKey(this Migration.IMigration migration)
        {
            return new Migration.MigrationKey(migration.SourceTypeVersionId, migration.TargetTypeVersionId);
        }

    }
}
