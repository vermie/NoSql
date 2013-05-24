using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoSqlWrapper.Serialization
{
    public interface ISerializerFactory
    {
        ISerializer<T> Get<T>();
    }
}
