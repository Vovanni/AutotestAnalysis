using AutotestAnalysis.Models;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
