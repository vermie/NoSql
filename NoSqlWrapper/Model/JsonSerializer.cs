using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NoSqlWrapper.Interfaces;

namespace NoSqlWrapper.Model
{
    internal class JsonSerializer<T> : ISerializer<T>
    {
        public Blob Serialize(T value)
        {
            var blobValue = JsonConvert.SerializeObject(value, Formatting.None);
            var blob = new Blob()
            {
                Value = blobValue,
                Metadata = typeof(T).FullName,
            };

            return blob;
        }

        public T Deserialize(Blob blob)
        {
            var value = JsonConvert.DeserializeObject<T>(blob.Value);
            return value;
        }
    }
}
