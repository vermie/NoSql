using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace NoSqlWrapper.Serialization
{
    internal class JsonSerializer<T> : ISerializer<T>
    {
        public bool StrictSerializationEnabled
        {
            get;
            set;
        }

        public JsonSerializerSettings DefaultJsonSettings
        {
            get;
            set;
        }

        public String Serialize(T value)
        {
            var serialized = JsonConvert.SerializeObject(value, Formatting.None, this.DefaultJsonSettings);


            return serialized;
        }      
        public T Deserialize(String value)
        {
            return JsonConvert.DeserializeObject<T>(value,this.DefaultJsonSettings);
        }

        public JsonSerializer()
        {
            this.DefaultJsonSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,//handles reference loops
                TypeNameHandling = TypeNameHandling.All,//needed for interfaces
                //ReferenceLoopHandling= ReferenceLoopHandling.Ignore,
            };
        }
    }

}
