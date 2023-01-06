using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace DTAClient.Domain.Multiplayer
{
    public class CustomMapCache
    {
        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("maps")]
        public ConcurrentDictionary<string, Map> Maps { get; set; }
    }
}
