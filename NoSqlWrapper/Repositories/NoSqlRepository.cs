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
                this.Context.AddTypeVersion(typeVersion);
                this.Context.SaveChanges();
            }

            return typeVersion;
        }
        private String GetTypeSignature<T>()
        {
            return this.TypeVersioner.GetTypeSignature<T>();
        }

        #region INoSqlRepository
        public Guid Create<T>(T instance)
        {
            var value = Serialize(instance);

            var typeVersion = this.ResolveTypeVersion<T>();

            var entity = this.Context.NewStore();
            entity.TypeVersionId = typeVersion.TypeVersionId;
            entity.Value = value;
            this.Context.AddStore(entity);

            return entity.StoreId;
        }

        public void Update<T>(Guid id, T instance)
        {
            var value = Serialize(instance);

            var typeVersion = this.ResolveTypeVersion<T>();

            var store = Context.Store.Find(id);
            if (store.TypeVersionId == typeVersion.TypeVersionId)
            {
                //update if the version is the same
                store.Value = value;
            }
            else
            {
                //insert if the version is different...but with same key
                var entity = this.Context.NewStore();
                entity.StoreId = id;
                entity.TypeVersionId = typeVersion.TypeVersionId;
                entity.Value = value;
                this.Context.AddStore(entity);
            }
        }

        public int Delete<T>(Guid id)
        {
            //delete ALL versions at this point
            var stores = Context.FindStore(id);
            if (stores.Any())
            {
                Context.DeleteStore(stores.ToArray());
            }

            return stores.Count();
        }

        public T Retrieve<T>(Guid id)
        {
            var typeVersion = this.ResolveTypeVersion<T>();

            var stores = Context.FindStore(id);

            var store = Context.Store.Find(id,typeVersion.TypeVersionId);

            if (store == null)
                return default(T);

            // TODO: apply migrations to blob
            var instance = Deserialize<T>(store.Value);

            return instance;
        }

        public T Retrieve<T>(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }
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

        public int Delete(Guid id)
        {
            return _store.Delete<T>(id);
        }

        public T Retrieve(Guid id)
        {
            return _store.Retrieve<T>(id);
        }

        public T Retrieve(Expression<Func<T, bool>> expression)
        {
            return _store.Retrieve(expression);
        }
    }

}
