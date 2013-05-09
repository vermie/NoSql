using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NoSqlWrapper.Interfaces;

namespace NoSqlWrapper.Model
{
    internal class DefaultSerializerFactory : ISerializerFactory
    {
        public ISerializer<T> Get<T>()
        {
            return new JsonSerializer<T>();
        }
    }
}
