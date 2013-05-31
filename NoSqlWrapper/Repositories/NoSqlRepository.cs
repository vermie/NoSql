using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

        protected Guid? TryFindKeyByConvention<T>(T instance)
        {
            var type = typeof(T);
            PropertyInfo keyProperty=null;

            //look for attribute first!
            var properties = NoSqlKeyAttribute.FindKeyProperty<T>();
            if (properties.Length == 1)
            {
                if (properties[0].PropertyType != typeof(Guid))
                {
                    throw new Exceptions.NoSqlWrapperException("NoSqlKeyAttribute must be defined on a property of type Guid");
                }

                keyProperty = properties[0];
            }
            if (properties.Length > 1)
            {
                throw new Exceptions.NoSqlWrapperException(String.Format(
                    "More than one NoSqlKeyAttribute is defined for type {0}",type.FullName));
            }

            //convention 2...try by class name + id
            keyProperty = type.GetProperty(type.Name + "Id", typeof(Guid));

            //last convention...just look for Id
            if (keyProperty == null)
                keyProperty = type.GetProperty("Id", typeof(Guid));

            if (keyProperty != null)
            {
                return (Guid)keyProperty.GetValue(instance, null);
            }

            return null;
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
            var serializer = SerializerFactory.Get<T>(this.Options);
            return serializer.Serialize(instance);
        }
        private T Deserialize<T>(String value)
        {
            var serializer = SerializerFactory.Get<T>(this.Options);
            return serializer.Deserialize(value);
        }

        private ITypeVersionEntity ResolveTypeVersion<T>()
        {
            //get a custom type version if its defined (do THIS first)
            ITypeVersionEntity typeVersion = this.TryResolveCustomTypeVersion<T>();

            //if not defined, look for an existing type version
            if (typeVersion == null)
            {
                //look in cache at some point...
                var typeName = typeof(T).FullName;
                var assemblyName = typeof(T).Assembly.GetName().Name;
                var typeSignature = this.GetTypeSignature<T>();
                typeVersion = this.Context.TryFindTypeVersion(assemblyName, typeName, typeSignature);
            }

            //if we are still null, add the type version
            if (typeVersion == null)
            {
                typeVersion = this.Context.NewTypeVersion();
                typeVersion.AssemblyName = typeof(T).Assembly.GetName().Name;
                typeVersion.TypeName = typeof(T).FullName;
                typeVersion.TypeSignature = this.GetTypeSignature<T>();
                typeVersion.DateCreated = this.Options.DateTimeProvider.Now;
                this.Context.AddTypeVersion(typeVersion);
                //this.Context.SaveChanges();
            }

            return typeVersion;
        }
        private ITypeVersionEntity TryResolveCustomTypeVersion<T>()
        {
            var typeSignatureAttribute = typeof(T).GetCustomAttribute<TypeSignatureAttribute>();
            ITypeVersionEntity typeVersion = null;

            if (typeSignatureAttribute != null)
            {
                typeVersion = this.Context.TryFindTypeVersion(typeSignatureAttribute.TypeVersionId);

                if (typeVersion == null)
                {
                    typeVersion = this.Context.NewTypeVersion();
                    typeVersion.AssemblyName = typeof(T).Assembly.GetName().Name;
                    typeVersion.DateCreated = this.Options.DateTimeProvider.Now;
                    typeVersion.TypeName = typeof(T).FullName;
                    typeVersion.TypeSignature = typeSignatureAttribute.TypeSignature;
                    typeVersion.TypeVersionId = typeSignatureAttribute.TypeVersionId;
                    this.Context.AddTypeVersion(typeVersion);
                }
                else
                {
                    if (String.Compare(typeVersion.TypeSignature, typeSignatureAttribute.TypeSignature, false) != 0)
                    {
                        //need better exception here..
                        throw new Exceptions.NoSqlWrapperException("Type signature does not match!");
                    }
                }
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

        private String ApplyMigrations<T>(IStoreEntity storeEntity)
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

            //archive if enabled and typeversion is different
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

            //update the living object
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
            var finalBlob = this.ApplyMigrations<T>(store);

            //deserialize the blob now
            var instance = Deserialize<T>(finalBlob);

            return instance;
        }

        #endregion
    }


}
