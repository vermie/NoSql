using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NoSqlWrapper.Interfaces
{
    public interface IStore
    {
        Guid Create<T>(T instance);
        void Update<T>(Guid id, T instance);
        int Delete<T>(Guid id);
        T Retrieve<T>(Guid id);
        T Retrieve<T>(Expression<Func<T, bool>> expression);
    }
}
