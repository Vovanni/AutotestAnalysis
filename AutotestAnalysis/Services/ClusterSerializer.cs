using AutotestAnalysis.Models;
using Newtonsoft.Json;
using Serilog;
using System.Collections.Generic;
using System.Linq;

namespace AutotestAnalysis.Services
{
    public class ClusterSerializer : IClusterSerializer
    {
        public string Serialize(List<Cluster> clusters)
        {
            var denrogram = clusters.Select(s => s.GetDendrogram());
            var serialized = JsonConvert.SerializeObject(denrogram);
            Log.Verbose("Serialized json: {json}", serialized);
            return serialized;
        }
    }
}
