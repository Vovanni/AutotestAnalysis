using AutotestAnalysis.Models;
using System.Collections.Generic;

namespace AutotestAnalysis.Services
{
    public interface IClusterSerializer
    {
        string Serialize(List<Cluster> clusters);
    }
}
