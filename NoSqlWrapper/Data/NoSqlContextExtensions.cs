using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoSqlWrapper.Data.Entity;

namespace NoSqlWrapper.Data
{

    internal static class NoSqlContextExtensions
    {
      
        public static IStoreEntity NewStore(this NoSQLContext context)
        {
            return new StoreEntity() { StoreId = Guid.NewGuid() };
        }
        public static void AddStore(this NoSQLContext context,IStoreEntity storeEntity)
        {
            var item = context.Store.Create();
            context.Store.Add(item);

            item.StoreId = storeEntity.StoreId;
            item.TypeVersionId = storeEntity.TypeVersionId;
            item.Value = storeEntity.Value;
        }

        public static IEnumerable<IStoreEntity> FindStore(this NoSQLContext context, Guid storeId)
        {
            return context.Store.Where(a => a.StoreId == storeId).ToArray();
        }
        public static IStoreEntity TryFindStore(this NoSQLContext context, Guid storeId, Guid typeVersionId)
        {
            return context.Store.Where(a => a.StoreId == storeId && a.TypeVersionId == typeVersionId).FirstOrDefault();
        }
        public static Int32 DeleteStore(this NoSQLContext context, params IStoreEntity[] storeItems)
        {
            foreach (var item in storeItems)
            {
                context.Store.Remove(item as StoreEntity);
            }

            return storeItems.Length;
        }

        public static ITypeVersionEntity NewTypeVersion(this NoSQLContext context)
        {
            return new TypeVersionEntity() { TypeVersionId = Guid.NewGuid() };
        }
        public static void AddTypeVersion(this NoSQLContext context, ITypeVersionEntity typeVersionEntity)
        {
            var item = context.TypeVersion.Create();
            context.TypeVersion.Add(item);

            item.AssemblyName = item.AssemblyName;
            item.TypeName = typeVersionEntity.TypeName;
            item.TypeSignature = typeVersionEntity.TypeSignature;
            item.TypeVersionId = typeVersionEntity.TypeVersionId;
            item.AssemblyName = typeVersionEntity.AssemblyName;
        }

        public static ITypeVersionEntity TryFindTypeVersion(this NoSQLContext context,String assemblyName, String typeName, String signature)
        {
            var item = context.TypeVersion
                .Where(a => a.TypeName == typeName && a.AssemblyName == a.AssemblyName && a.TypeSignature == signature).FirstOrDefault();

            return item as ITypeVersionEntity;
        }


    }

}
