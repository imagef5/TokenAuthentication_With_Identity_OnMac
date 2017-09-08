using System;
using Newtonsoft.Json;

namespace ApiClient.DataServices.Base
{
    public interface IJsonSettings
    {
        JsonSerializerSettings JsonSerializerSettings { get; }
    }
}
