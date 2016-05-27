using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace System.WebAPIProxy.Serialization
{
    /// <summary>
    /// Custom converter to convert objects to and from JSON
    /// </summary>
    /// <typeparam name="T">The type of object being passed in</typeparam>
    public abstract class CustomJsonConverter<T> : JsonConverter
    {
        /// <summary>
        /// Abstract method which implements the appropriate create method
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        protected abstract T Create(Type objectType, JObject jsonObject);

        /// <summary>
        /// Determines whether an instance of the current System.Type can be assigned from an instance of the specified Type.
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        /// <summary>
        /// Reads JSON and returns the appropriate object
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // load the json string
            var jsonObject = JObject.Load(reader);

            // instantiate the appropriate object based on the json string
            var target = Create(objectType, jsonObject);

            // populate the properties of the object
            var read = jsonObject.CreateReader();
            serializer.Populate(read, target);

            // return the object
            return target;
        }

        /// <summary>
        /// Creates the JSON based on the object passed in
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class JsonNetSerialization : ISerialization
    {
        public JsonNetSerialization() { }
        JsonSerializerSettings jsonSerializerSettings = null;
        public JsonNetSerialization(JsonSerializerSettings settings) { jsonSerializerSettings = settings; }

        public string Serialize<T>(object obj)
        {
            return JsonConvert.SerializeObject((T)obj);
        }
        public string Serialize<T>(object obj, bool ApplySerializeSettings)
        {
            if (ApplySerializeSettings)
                return JsonConvert.SerializeObject((T)obj, jsonSerializerSettings);
            else
                return JsonConvert.SerializeObject((T)obj);
        }
        public TDestination Serialize<TSource, TDestination>(TSource source)
        {
            try
            {
                var json = this.Serialize<TSource>(source, true);
                return JsonConvert.DeserializeObject<TDestination>(json, jsonSerializerSettings);
            }
            catch { }
            TDestination t = default(TDestination);
            return t;
        }
        public T DeSerialize<T>(System.IO.Stream stream)
        {
            return JsonConvert.DeserializeObject<T>(new StreamReader(stream).ReadToEnd());
        }
        public T DeSerialize<T>(string value)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(value, jsonSerializerSettings);
            }
            catch { }
            T t = default(T);
            return t;            
        }
    }
}
