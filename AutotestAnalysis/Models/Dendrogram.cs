using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutotestAnalysis.Models
{
    public class Dendrogram
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("children", NullValueHandling = NullValueHandling.Ignore)]
        public List<Dendrogram> Children { get; set; }

        public Dendrogram()
        {
        }
    }
}
