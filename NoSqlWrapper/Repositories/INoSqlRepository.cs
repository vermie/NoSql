using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NoSqlWrapper.Repositories
{
    public interface INoSqlRepository
    {
        Guid Create<T>(T instance);
        void Update<T>(Guid id, T instance);
        void Delete<T>(Guid id);
        T TryRetrieve<T>(Guid id);
        //T Retrieve<T>(Expression<Func<T, bool>> expression);
    }

    
}
