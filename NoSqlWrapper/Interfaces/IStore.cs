using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NoSqlWrapper.Interfaces
{
    public interface IStoreWithKey<TKey>
    {
        TKey Create<T>(T instance);
        void Update<T>(TKey id, T instance);
        int Delete<T>(TKey id);
        T Retrieve<T>(TKey id);
        T Retrieve<T>(Expression<Func<T, bool>> expression);
    }
    public interface IStore : IStoreWithKey<Guid>
    {
    }
}
