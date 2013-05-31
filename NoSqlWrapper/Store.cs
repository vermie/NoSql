using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NoSqlWrapper.Conventions;
using NoSqlWrapper.Data;
using NoSqlWrapper.Repositories;
using NoSqlWrapper.Serialization;
using NoSqlWrapper.Versioning;

namespace NoSqlWrapper
{
    //this should be the facade
    public partial class Store : NoSqlRepository,IStore, IDisposable
    {
        private readonly NoSQLContext _context;

        public Store()
            : this(new NoSQLContext())
        {

        }
        private Store(NoSQLContext context)
            : this(context,new Options(), new DefaultSerializerFactory(), new TypeVersioner())
        {

        }
        private Store(NoSQLContext context, Options options, ISerializerFactory serializerFactory, ITypeVersioner versioner)
            : base(context, options, serializerFactory, versioner)
        {
            this._context = context;
        }

        public void SaveChanges()
        {
            this._context.SaveChanges();
        }
        #region IDisposable implemented
        private bool disposed = false; // to detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    this._context.TryDispose();
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
