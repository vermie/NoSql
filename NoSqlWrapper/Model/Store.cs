using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NoSqlWrapper.Data;
using NoSqlWrapper.Interfaces;

namespace NoSqlWrapper.Model
{
    public class Store : IStore
    {
        private NoSQLContext _context;

        public Store(NoSQLContext context)
        {
            _context = context;
        }

        public Guid Create<T>(T instance)
        {
            var result = _context.Store.Create();
            result.Id = Guid.NewGuid();
            result.Value = instance;
            _context.Store.Add(result);

            return result.Id;
        }

        public void Update<T>(Guid id, T instance)
        {
            var store = _context.Store.Find(id);
            store.Value = instance;
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
            if (store == null) { return default(T); }

            return (T)store.Value;
        }

        public T Retrieve<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }
    }

    public class Store<T> : IStore<T>
    {
        private IStore _store;

        public Store(NoSQLContext context)
        {
            _store = new Store(context);
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

        public T Retrieve(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            return _store.Retrieve(expression);
        }
    }
}
