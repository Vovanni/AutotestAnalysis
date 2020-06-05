using AutotestAnalysis.Models;
using Serilog;
using System.Collections.Generic;
using System.Linq;

namespace AutotestAnalysis.Services
{
    public class HierarchicalClustering : IHierarchicalClustering
    {
        private List<Cluster> _outputCluster;
        public List<Cluster> Output => _outputCluster;

        public HierarchicalClustering()
        {
            _outputCluster = new List<Cluster>();
        }

        public void Clear()
        {
            _outputCluster.Clear();
        }

        public void Compute(float fitness, IEnumerable<Cluster> clusters)
        {
            if (!clusters.Any())
            {
                return;
            }

            var clusterQueue = new Queue<Cluster>(clusters);
            var clusterBuffer = new Queue<Cluster>();

            Cluster a = clusterQueue.Dequeue();
            Cluster b = null;
            var bestFitness = fitness;
            while (clusterQueue.Count > 0)
            {
                //Получаем элемент из очереди
                var c = clusterQueue.Dequeue();
                var computedFitness = Cluster.ComputeFitness(new List<Cluster> { a, c });
                if (computedFitness <= bestFitness)
                {
                    if (!(b is null))
                    {
                        clusterBuffer.Enqueue(b);
                    }

                    bestFitness = computedFitness;
                    b = c;
                }
                else
                {
                    clusterBuffer.Enqueue(c);
                }
            }

            if (b is null)
            {
                _outputCluster.Add(a);
            }
            else
            {
                clusterBuffer.Enqueue(new Cluster(new List<Cluster> { a, b }));
            }

            Compute(fitness, clusterBuffer);
        }

        public void ComputeMultiple(float fitness, IEnumerable<Cluster> clusters)
        {
            if (!clusters.Any())
            {
                return;
            }

            var clusterQueue = new Queue<Cluster>(clusters);
            var clusterBuffer = new Queue<Cluster>();

            var a = clusterQueue.Dequeue();
            var merge = new List<Cluster>();
            var bestFitness = fitness;
            while (clusterQueue.Count > 0)
            {
                //Получаем элемент из очереди
                var c = clusterQueue.Dequeue();
                var pairFitness = Cluster.ComputeFitness(new List<Cluster> { a, c });
                if (pairFitness <= bestFitness)
                {
                    var mergeBuffer = new List<Cluster>(merge);
                    mergeBuffer.Add(a);
                    mergeBuffer.Add(c);
                    var groupFitness = Cluster.ComputeFitness(mergeBuffer);

                    if (groupFitness <= pairFitness)
                    {
                        //Добавить в группу
                        bestFitness = groupFitness;
                    }
                    else
                    {
                        merge.ForEach(s => clusterBuffer.Enqueue(s));
                        merge.Clear();
                        bestFitness = pairFitness;
                    }

                    merge.Add(c);
                }
                else
                {
                    clusterBuffer.Enqueue(c);
                }
            }

            if (!merge.Any())
            {
                _outputCluster.Add(a);
            }
            else
            {
                merge.Add(a);
                Log.Debug("Merged cluster: {cluster}", new Cluster(merge));
                clusterBuffer.Enqueue(new Cluster(merge));
            }

            ComputeMultiple(fitness, clusterBuffer);
        }
    }
}
