using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoSqlWrapper.Serialization
{
    internal class DefaultSerializerFactory : ISerializerFactory
    {
        public ISerializer<T> Get<T>()
        {
            return new JsonSerializer<T>();
        }
    }
}
