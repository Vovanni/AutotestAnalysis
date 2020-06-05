using System.Linq;
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
            var sessions = JArray.Parse(System.IO.File.ReadAllText(@"D:\User\Desktop\response_1589728162482.json"));

            var results = ParserManager.ParseTestResults(sessions);
            HierarchicalClustering.ComputeMultiple(0.5f, results.Clusters);
            Log.Information("Tests count: {tcount}, clusters count {ccount}, cluster depth: {depth}", 
                results.Clusters.Count, HierarchicalClustering.Output.Count, HierarchicalClustering.Output.Max(c => c.Depth));

            ViewBag.Width = HierarchicalClustering.Output.Max(c => c.Depth) * 150 + 460;
            ViewBag.Height = results.Clusters.Count * 30;
            ViewBag.Cluster = ClusterSerializer.Serialize(HierarchicalClustering.Output);
            return View();
        }
    }
}