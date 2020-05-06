using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutotestAnalysis.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

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
            HierarchicalClustering.Compute(0.5f, results.Clusters);
            ViewBag.Cluster = ClusterSerializer.Serialize(HierarchicalClustering.Output);
            return View();
        }
    }
}