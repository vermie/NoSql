using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
            item.DateCreated = storeEntity.DateCreated;
            item.LastUpdated = storeEntity.LastUpdated;
        }
        public static void UpdateStore(this NoSQLContext context, IStoreEntity storeEntity)
        {
            var entity = context.Store.Find(storeEntity.StoreId);

            entity.DateCreated = storeEntity.DateCreated;
            entity.LastUpdated = storeEntity.LastUpdated;
            entity.TypeVersionId = storeEntity.TypeVersionId;
            entity.Value = storeEntity.Value;
        }


        public static IStoreArchiveEntity NewStoreArchive(this NoSQLContext context)
        {
            return new StoreArchiveEntity();
        }
        public static void AddStoreArchive(this NoSQLContext context, IStoreArchiveEntity storeEntity)
        {
            var item = context.StoreArchive.Create();
            context.StoreArchive.Add(item);

            item.LastUpdated = storeEntity.LastUpdated;
            item.DateArchived = storeEntity.DateArchived;
            item.DateCreated = storeEntity.DateCreated;
            item.StoreArchiveId = storeEntity.StoreArchiveId;
            item.StoreId = storeEntity.StoreId;
            item.TypeVersionId = storeEntity.TypeVersionId;
            item.Value = storeEntity.Value;
        }

        public static IStoreEntity TryFindStore(this NoSQLContext context, Guid storeId)
        {
            return context.Store.Where(a => a.StoreId == storeId).FirstOrDefault();
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
            item.DateCreated = typeVersionEntity.DateCreated;
        }

        public static ITypeVersionEntity TryFindTypeVersion(this NoSQLContext context, Guid typeVersionId)
        {
            Func<TypeVersionEntity, Boolean> delegateSearch =
                a => a.TypeVersionId == typeVersionId ;

            //search local first...
            var item = context.TypeVersion.Local.Where(delegateSearch).FirstOrDefault();

            //now go search against real db
            if (item == null)
            {
                item = context.TypeVersion.Where(delegateSearch).FirstOrDefault();
            }

            return item as ITypeVersionEntity;
        }
        public static ITypeVersionEntity TryFindTypeVersion(this NoSQLContext context,String assemblyName, String typeName, String signature)
        {
            Func<TypeVersionEntity,Boolean> delegateSearch = 
                a => a.TypeName == typeName && a.AssemblyName == a.AssemblyName && a.TypeSignature == signature;

            //search local first...
            var item = context.TypeVersion.Local.Where(delegateSearch).FirstOrDefault();

            //now go search against real db
            if (item == null)
            {
                item = context.TypeVersion.Where(delegateSearch).FirstOrDefault();
            }

            return item as ITypeVersionEntity;
        }


    }

}
