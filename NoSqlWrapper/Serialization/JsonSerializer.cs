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
        private JsonSerializerSettings _jsonSettings;
        public Conventions.Options Options
        {
            get;
            set;
        }

        public JsonSerializerSettings JsonSettings
        {
            get
            {
                if (this._jsonSettings == null)
                {
                    this._jsonSettings = new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,//handles reference loops
                        TypeNameHandling = TypeNameHandling.All,//needed for interfaces
                        //ReferenceLoopHandling= ReferenceLoopHandling.Ignore,
                        MissingMemberHandling = this.Options.StrictDeserializationEnabled ? MissingMemberHandling.Error : MissingMemberHandling.Ignore
                    };
                }

                return this._jsonSettings;
            }
            set
            {
                this._jsonSettings = value;
            }
        }

        public String Serialize(T value)
        {
            try
            {
                var serialized = JsonConvert.SerializeObject(value, Formatting.None, this.JsonSettings);

                return serialized;
            }
            catch (JsonException jex)
            {
                throw new Exceptions.NoSqlSerializationException(
                    String.Format("Failed to serialize object '{0}'. See inner exception for details.", typeof(T).FullName), jex);
            }
        }      
        public T Deserialize(String value)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(value, this.JsonSettings);
            }
            catch (JsonException  jex)
            {
                throw new Exceptions.NoSqlSerializationException(
                    String.Format("Failed to deserialize object '{0}'. See inner exception for details.",typeof(T).FullName), jex);
            }
        }

        public JsonSerializer()
        {

        }


        
    }

}
