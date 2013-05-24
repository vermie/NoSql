using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NoSqlWrapper.Conventions;
using NoSqlWrapper.Data;
using NoSqlWrapper.Data.Entity;
using NoSqlWrapper.Serialization;
using NoSqlWrapper.Versioning;

namespace NoSqlWrapper.Repositories
{
    
    public class NoSqlRepository : INoSqlRepository
    {
        private NoSQLContext Context
        {
            get;
            set;
        }
        private ISerializerFactory SerializerFactory
        {
            get;
            set;
        }
        private ITypeVersioner TypeVersioner
        {
            get;
            set;
        }
        public Options Options
        {
            get;
            set;
        }
        public NoSqlRepository(NoSQLContext context)
            : this(context, new Options(), new DefaultSerializerFactory(), new TypeVersioner())
        {
        }

        public NoSqlRepository(NoSQLContext context,Options options, ISerializerFactory serializerFactory,ITypeVersioner versioner)
        {
            //set the external context
            this.Context = context;

            this.Options = options;
            SerializerFactory = serializerFactory;
            TypeVersioner = versioner;
        }

        private String Serialize<T>(T instance)
        {
            var serializer = SerializerFactory.Get<T>();
            return serializer.Serialize(instance);
        }
        private T Deserialize<T>(String value)
        {
            var serializer = SerializerFactory.Get<T>();
            return serializer.Deserialize(value);
        }

        private ITypeVersionEntity ResolveTypeVersion<T>()
        {
            //look in cache at some point...
            var typeName = typeof(T).FullName;
            var assemblyName = typeof(T).Assembly.GetName().Name;
            var typeSignature = this.GetTypeSignature<T>();

            ITypeVersionEntity typeVersion = this.Context.TryFindTypeVersion(assemblyName,typeName, typeSignature);
            if (typeVersion == null)
            {
                typeVersion = this.Context.NewTypeVersion();
                typeVersion.AssemblyName = assemblyName;
                typeVersion.TypeName = typeName;
                typeVersion.TypeSignature = typeSignature;
                typeVersion.DateCreated = this.Options.DateTimeProvider.Now;
                this.Context.AddTypeVersion(typeVersion);
                //this.Context.SaveChanges();
            }

            return typeVersion;
        }
        private String GetTypeSignature<T>()
        {
            if (this.Options.VersioningEnabled)
            {
                return this.TypeVersioner.GetTypeSignature<T>();
            }
            else
            {
                return String.Empty;
            }
        }

        private String ResolveMigrations<T>(IStoreEntity storeEntity)
        {
            //short circuit if migrations are disabled??

            //find the type version for what we have loaded
            var typeVersion = this.ResolveTypeVersion<T>();

            //version mismatch, apply a migration strategy (use decorator???)
            if (typeVersion.TypeVersionId != storeEntity.TypeVersionId)
            {
                //TODO
            }

            //for now just return this JSON
            return storeEntity.Value;
        }

        #region INoSqlRepository
        public Guid Create<T>(T instance)
        {
            var value = Serialize(instance);

            var typeVersion = this.ResolveTypeVersion<T>();

            var entity = this.Context.NewStore();
            entity.TypeVersionId = typeVersion.TypeVersionId;
            entity.Value = value;
            entity.DateCreated = this.Options.DateTimeProvider.Now;
            entity.LastUpdated = entity.DateCreated;
            this.Context.AddStore(entity);

            return entity.StoreId;
        }

        public void Update<T>(Guid id, T instance)
        {
            var value = Serialize(instance);
            var typeVersion = this.ResolveTypeVersion<T>();

            var store = Context.Store.Find(id);

            //store this version
            if ((store.TypeVersionId != typeVersion.TypeVersionId) &&
                (this.Options.ArchiveVersionChanges))
            {
                //update if the version is the same
                var archive = this.Context.NewStoreArchive();
                archive.DateArchived = this.Options.DateTimeProvider.Now;
                archive.DateCreated = store.DateCreated;
                archive.LastUpdated = store.LastUpdated;
                archive.StoreArchiveId = Guid.NewGuid();
                archive.StoreId = store.StoreId;
                archive.TypeVersionId = store.TypeVersionId;
                archive.Value = store.Value;

                this.Context.AddStoreArchive(archive);
            }

            store.TypeVersionId = typeVersion.TypeVersionId;
            store.Value = value;
            store.LastUpdated = this.Options.DateTimeProvider.Now;

            this.Context.UpdateStore(store);
        }

        public void Delete<T>(Guid id)
        {
            //delete ALL versions at this point
            var stores = Context.TryFindStore(id);
            if (stores != null)
            {
                Context.DeleteStore(stores);
            }
        }

        public T TryRetrieve<T>(Guid id)
        {
            //try to find the item
            var store = Context.TryFindStore(id);

            //if we cannot find it return default
            if (store == null)
                return default(T);

            //resolve any migrations here.
            var finalBlob = this.ResolveMigrations<T>(store);

            // TODO: apply migrations to blob
            var instance = Deserialize<T>(finalBlob);

            return instance;
        }

        //public T Retrieve<T>(Expression<Func<T, bool>> expression)
        //{
        //    throw new NotImplementedException();
        //}
        #endregion
    }

    public class NoSqlRepository<T> : INoSqlRepository<T>
    {
        private readonly INoSqlRepository _store;

        public NoSqlRepository(NoSQLContext context)
            : this(context,new Options(), new DefaultSerializerFactory(), new TypeVersioner())
        {
        }

        public NoSqlRepository(NoSQLContext context, Options options, ISerializerFactory serializerFactory, ITypeVersioner versioner)
        {
            _store = new NoSqlRepository(context, options, serializerFactory, versioner);
        }

        public Guid Create(T instance)
        {
            return _store.Create(instance);
        }

        public void Update(Guid id, T instance)
        {
            _store.Update(id, instance);
        }

        public void Delete(Guid id)
        {
             _store.Delete<T>(id);
        }

        public T Retrieve(Guid id)
        {
            return _store.TryRetrieve<T>(id);
        }
    }

}
