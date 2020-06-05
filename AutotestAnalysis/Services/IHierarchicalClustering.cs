using AutotestAnalysis.Models;
using System.Collections.Generic;

namespace AutotestAnalysis.Services
{
    public interface IHierarchicalClustering
    {
        List<Cluster> Output { get; }
        void Compute(float fitness, IEnumerable<Cluster> clusters);
        void ComputeMultiple(float fitness, IEnumerable<Cluster> clusters);
        void Clear();
    }
}
