using AutotestAnalysis.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Frameworks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
    [TestClass]
    public class ClusterTests
    {
        private Cluster Cluster1 = new Cluster("A", "win-7", "message A", new Dictionary<int, int>
        {
            { 0, 1 },
            { 1, 2 },
            { 3, 2 },
            { 4, 4 }
        });

        private Cluster Cluster2 = new Cluster("A", "win-7", "message A", new Dictionary<int, int>
        {
            { 0, 1 },
            { 1, 1 },
            { 2, 2 },
            { 3, 2 }
        });

        [TestMethod]
        public void ClusterMerge()
        {
            var mergedCluster = new Cluster(new List<Cluster> { Cluster1, Cluster2 });
            Assert.AreEqual(mergedCluster.Fitness, 7);
            Console.Write(string.Join(",\n", mergedCluster.Tags.Select(s => $"{s.Key}: {s.Value}")));

            var testDict = new Dictionary<int, float>
            {
                { 0, 1 },
                { 1, 1.5f },
                { 3, 2 },
                { 4, 2 },
                { 2, 1 }
            };

            foreach (var tag in testDict)
            {
                Assert.IsTrue(mergedCluster.Tags.ContainsKey(tag.Key) && mergedCluster.Tags[tag.Key] == tag.Value);
            }

            Assert.IsFalse(mergedCluster.IsRoot);
        }

        [TestMethod]
        public void ClusterEqual()
        {
            Assert.IsTrue(Cluster1 == Cluster1);
            Assert.IsFalse(Cluster1 == Cluster2);
        }

        [TestMethod]
        public void ClusterNotEqual()
        {
            Assert.IsTrue(Cluster1 != Cluster2);
            Assert.IsFalse(Cluster2 != Cluster2);
        }

        [TestMethod]
        public void ClusterGetHashCode()
        {
            Assert.AreEqual(Cluster1.GetHashCode(), Cluster1.GetHashCode());
            Assert.AreNotEqual(Cluster1.GetHashCode(), Cluster2.GetHashCode());
        }

        [TestMethod]
        public void ClusterEquals()
        {
            Assert.IsFalse(Cluster1.Equals(null));
            Assert.IsTrue(Cluster1.Equals(Cluster1));
        }
    }
}
