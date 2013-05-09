using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NoSqlWrapper.Interfaces
{
    public interface IStoreWithKey<TKey, T>
    {
        TKey Create(T instance);
        void Update(TKey id, T instance);
        int Delete(TKey id);
        T Retrieve(TKey id);
        T Retrieve(Expression<Func<T, bool>> expression);
    }
    public interface IStore<T> : IStoreWithKey<Guid, T>
    {
    }
}
