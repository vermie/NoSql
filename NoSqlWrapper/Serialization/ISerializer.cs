using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoSqlWrapper.Serialization
{
    public interface ISerializer<T>
    {
        Boolean StrictSerializationEnabled { get; set; }

        String Serialize(T value);
        T Deserialize(String blob);
    }
}
