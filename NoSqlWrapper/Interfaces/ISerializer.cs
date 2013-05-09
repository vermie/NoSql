using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NoSqlWrapper.Model;

namespace NoSqlWrapper.Interfaces
{
    public interface ISerializer<T>
    {
        Blob Serialize(T value);
        T Deserialize(Blob blob);
    }
}
