//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 基于Json.Net实现的ISerializer
    /// </summary>
    public class JsonNetSerializer : ISerializer
    {
        private JsonSerializerSettings _settings = new JsonSerializerSettings { ContractResolver = new SisoJsonDefaultContractResolver() };

        public string Serialize(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            return JsonConvert.SerializeObject(obj, Formatting.None, new IsoDateTimeConverter());
        }
        public object Deserialize(string value, Type objectType)
        {
            return JsonConvert.DeserializeObject(value, objectType, _settings);
        }
        public T Deserialize<T>(string value) where T : class
        {
            return JsonConvert.DeserializeObject<T>(JObject.Parse(value).ToString(), _settings);
        }
        public T Deserialize<T>(string value, T anonymousTypeObject)
        {
            return JsonConvert.DeserializeAnonymousType<T>(JObject.Parse(value).ToString(), anonymousTypeObject);
        }
    }

    public class SisoJsonDefaultContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);

            if (!prop.Writable)
            {
                var property = member as PropertyInfo;
                if (property != null)
                {
                    var hasPrivateSetter = property.GetSetMethod(true) != null;
                    prop.Writable = hasPrivateSetter;
                }
            }

            return prop;
        }
    }
}
