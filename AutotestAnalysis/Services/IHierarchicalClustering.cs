using AutotestAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutotestAnalysis.Services
{
    public interface IHierarchicalClustering
    {
        List<Cluster> Output { get; }
        void Compute(float fitness, IEnumerable<Models.Cluster> clusters);
    }
}
