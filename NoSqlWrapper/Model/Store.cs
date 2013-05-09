using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NoSqlWrapper.Data;
using NoSqlWrapper.Entity;
using NoSqlWrapper.Interfaces;

namespace NoSqlWrapper.Model
{
    public class Store : IStore
    {
        private readonly NoSQLContext _context;
        private readonly ISerializerFactory _serializerFactory;

        public Store(NoSQLContext context)
            : this(context, new DefaultSerializerFactory())
        {
        }

        public Store(NoSQLContext context, ISerializerFactory serializerFactory)
        {
            _context = context;
            _serializerFactory = serializerFactory;
        }

        private Blob Serialize<T>(T instance)
        {
            var serializer = _serializerFactory.Get<T>();
            var blob = serializer.Serialize(instance);

            return blob;
        }

        private T Deserialize<T>(Blob blob)
        {
            var serializer = _serializerFactory.Get<T>();
            var instance = serializer.Deserialize(blob);

            return instance;
        }

        private StoreEntity MakeEntity(Blob blob)
        {
            return new StoreEntity()
            {
                Value = blob.Value,
                TypeName = blob.Metadata,
            };
        }

        private Blob MakeBlob(StoreEntity entity)
        {
            return new Blob()
            {
                Value = entity.Value,
                Metadata = entity.TypeName,
            };
        }

        public Guid Create<T>(T instance)
        {
            var blob = Serialize(instance);

            var result = MakeEntity(blob);
            result.Id = Guid.NewGuid();

            _context.Store.Add(result);

            return result.Id;
        }

        public void Update<T>(Guid id, T instance)
        {
            var blob = Serialize(instance);

            var store = _context.Store.Find(id);
            store.Value = blob.Value;
            store.TypeName = blob.Metadata;
        }

        public int Delete<T>(Guid id)
        {
            var store = _context.Store.Find(id);
            if (store == null) { return 0; }

            var result = _context.Store.Remove(store);
            return 1;
        }

        public T Retrieve<T>(Guid id)
        {
            var store = _context.Store.Find(id);

            if (store == null)
                return default(T);

            var blob = MakeBlob(store);
            // TODO: apply migrations to blob
            var instance = Deserialize<T>(blob);

            return instance;
        }

        public T Retrieve<T>(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }
    }

    public class Store<T> : IStore<T>
    {
        private readonly IStore _store;

        public Store(NoSQLContext context)
            : this(context, new DefaultSerializerFactory())
        {
        }

        public Store(NoSQLContext context, ISerializerFactory serializerFactory)
        {
            _store = new Store(context, serializerFactory);
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
