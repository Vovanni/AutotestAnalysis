using Newtonsoft.Json;
using System.Collections.Generic;

namespace AutotestAnalysis.Models
{
    public class Dendrogram
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("children", NullValueHandling = NullValueHandling.Ignore)]
        public List<Dendrogram> Children { get; set; }
    }
}
