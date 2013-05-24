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


        //public T JoseDeserialize(String json, JsonSerializerSettings settings)
        //{
        //    var value = JsonConvert.DeserializeObject<T>(json, settings);
        //    return value;
        //}
        //public String Jose(Object someObject,JsonConverter converter)
        //{


        //    using (var memoryStream = new MemoryStream())
        //    {
        //        var streamWriter = new StreamWriter(memoryStream);

        //        JsonWriter jsonWriter = new JsonTextWriter(streamWriter);

        //        JsonSerializer cereal = new JsonSerializer();
        //        cereal.ContractResolver = new MyResolver();
        //        cereal.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

        //        cereal.Converters.Add(converter);

        //        cereal.Serialize(jsonWriter, someObject);

        //        jsonWriter.Flush();


        //        memoryStream.Position = 0;
        //        Byte[] buffer = new Byte[memoryStream.Length];
        //        memoryStream.Read(buffer, 0, Convert.ToInt32(memoryStream.Length));
        //        return System.Text.ASCIIEncoding.UTF8.GetString(buffer);
        //    }

        //}

        
    }

    //public class MyResolver : DefaultContractResolver
    //{
    //    public override JsonContract ResolveContract(Type type)
    //    {
    //        var result = base.ResolveContract(type);

    //        return result;
    //        //return base.ResolveContract(type);
    //    }
    //}

    

}
