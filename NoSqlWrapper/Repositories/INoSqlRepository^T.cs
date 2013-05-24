using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NoSqlWrapper.Repositories
{
    //I am not sure what value this interface adds? I would rather see it go away kind of
    public interface INoSqlRepository<T>
    {
        Guid Create(T instance);
        void Update(Guid id, T instance);
        void Delete(Guid id);
        T Retrieve(Guid id);
        //T Retrieve(Expression<Func<T, bool>> expression);
    }
}
