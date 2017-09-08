using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace ApiClient.DataServices.Base
{
    internal class JsonSettings : IJsonSettings
    {
        public JsonSettings()
        {
            
        }
        private JsonSerializerSettings serializerSettings;
        public JsonSerializerSettings JsonSerializerSettings
        {
            get
            {
                serializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                    NullValueHandling = NullValueHandling.Ignore,
                };

                serializerSettings.Converters.Add(new StringEnumConverter());

                return serializerSettings;
            }
        }
    }
}
