using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NoSqlWrapper.Interfaces
{
    public interface IStore<T>
    {
        Guid Create(T instance);
        void Update(Guid id, T instance);
        int Delete(Guid id);
        T Retrieve(Guid id);
        T Retrieve(Expression<Func<T, bool>> expression);
    }
}
