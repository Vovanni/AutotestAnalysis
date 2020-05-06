using AutotestAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutotestAnalysis.Services
{
    public interface IClusterSerializer
    {
        string Serialize(List<Cluster> clusters);
    }
}
