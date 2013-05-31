using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoSqlWrapper.Conventions;
using NoSqlWrapper.Data;
using NoSqlWrapper.Serialization;
using NoSqlWrapper.Versioning;

namespace NoSqlWrapper.Repositories
{


    public class NoSqlRepository<T> : INoSqlRepository<T>
    {
        private readonly INoSqlRepository _store;

        public NoSqlRepository(NoSQLContext context)
            : this(context, new Options(), new DefaultSerializerFactory(), new TypeVersioner())
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

        public void Delete(Guid id)
        {
            _store.Delete<T>(id);
        }

        public T Retrieve(Guid id)
        {
            return _store.TryRetrieve<T>(id);
        }
    }

}
