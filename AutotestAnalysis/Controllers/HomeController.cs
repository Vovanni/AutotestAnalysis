using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutotestAnalysis.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Serilog;

namespace AutotestAnalysis.Controllers
{
    public class HomeController : Controller
    {
        private IParserManager ParserManager { get; }
        private IHierarchicalClustering HierarchicalClustering { get; }
        private IClusterSerializer ClusterSerializer { get; }

        public HomeController(IParserManager parserManager, 
            IHierarchicalClustering hierarchicalClustering, 
            IClusterSerializer clusterSerializer)
        {
            ParserManager = parserManager;
            HierarchicalClustering = hierarchicalClustering;
            ClusterSerializer = clusterSerializer;
        }

        public IActionResult Index()
        {
            var sessions = JArray.Parse(System.IO.File.ReadAllText(@"D:\User\Desktop\response_1588673196920.json"));

            var results = ParserManager.ParseTestResults(sessions);
            HierarchicalClustering.ComputeMultiple(0.4f, results.Clusters);
            Log.Debug("Tests count: {count}, cluster depth: {depth}", results.Clusters.Count, HierarchicalClustering.Output.Max(c => c.Depth));
            
            ViewBag.Width = HierarchicalClustering.Output.Max(c => c.Depth) * 300 + 460;
            ViewBag.Height = results.Clusters.Count * 30;
            ViewBag.Cluster = ClusterSerializer.Serialize(HierarchicalClustering.Output);
            return View();
        }
    }
}