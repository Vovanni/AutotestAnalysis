using Microsoft.AspNetCore.Razor.TagHelpers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Policy;
using System.Threading.Tasks;

namespace AutotestAnalysis.Models
{
    public class Cluster
    {
        public List<Cluster> Childs;

        //Проверяем что кластер является корнем
        public bool IsRoot => Childs == null || Childs.Count == 0;

        public Dictionary<int, float> Tags;

        public float Fitness { get; }

        public List<string> Products;
        public string Product => string.Join("-", Products);

        public List<string> Tests;
        public string Test => string.Join("-", Tests);

        public List<string> Platforms;
        public string Platform => string.Join("-", Platforms);

        public string Name => $"{Product}-{Test}-{Platform}";

        private string _message;
        public string Message => _message ?? string.Join("\n", Childs.Select(c => c.Message));

        public int Depth => IsRoot ? 1 : Childs.Max(c => c.Depth) + 1;

        public int Count => IsRoot ? 1 : Childs.Sum(c => c.Count);

        public Cluster(string product, string test, string platform, string message, Dictionary<int, int> tags)
        {
            Products = new List<string> { product };
            Tests = new List<string> { test };
            Platforms = new List<string> { platform };
            _message = message;
            Tags = new Dictionary<int, float>();
            foreach (var tag in tags)
            {
                Tags.Add(tag.Key, tag.Value);
            }
            //Log.Debug("Creating new cluster with message: {message}\ntags: {tags}", Message, Tags);
            Fitness = 0;
        }

        public Cluster(List<Cluster> clustersToMerge)
        {
            Childs = clustersToMerge;
            Tags = new Dictionary<int, float>();
            var tags = new Dictionary<int, float>();
            Fitness = 0;

            var products = new List<string>();
            var tests = new List<string>();
            var platforms = new List<string>();

            //Считаем сумму
            foreach (var cluster in clustersToMerge)
            {
                products.AddRange(cluster.Products);
                tests.AddRange(cluster.Tests);
                platforms.AddRange(cluster.Platforms);

                foreach (var tag in cluster.Tags)
                {
                    if (tags.ContainsKey(tag.Key))
                    {
                        tags[tag.Key] += tag.Value;
                    }
                    else
                    {
                        tags.Add(tag.Key, tag.Value);
                    }
                }
            }

            //Считаем среднее арифметическое
            foreach (var tag in tags)
            {
                Tags.Add(tag.Key, tag.Value / clustersToMerge.Count);
            }

            //Считаем приспособляемость
            foreach (var cluster in clustersToMerge)
            {
                foreach (var tag in Tags)
                {
                    //Прибавляем к приспособляемости отклонение от среднего арифметического для каждого кластера
                    Fitness += Math.Abs(tag.Value - (cluster.Tags.ContainsKey(tag.Key) ? cluster.Tags[tag.Key] : 0)) / cluster.Tags.Count;
                }
            }

            Products = products.Distinct().ToList();
            Tests = tests.Distinct().ToList();
            Platforms = platforms.Distinct().ToList();
        }

        public static float ComputeFitness(List<Cluster> clusters)
        {
            return new Cluster(clusters).Fitness;
        }

        public Dendrogram GetDendrogram()
        {
            var dendrogram = new Dendrogram { Name = Name };

            if (Childs == null || !Childs.Any())
            {
                //dendrogram.Name = Name;
                return dendrogram;
            }

            dendrogram.Children = new List<Dendrogram>();
            foreach (var child in Childs)
            {
                dendrogram.Children.Add(child.GetDendrogram());
            }

            return dendrogram;
        }

        #region Overrides

        public static bool operator !=(Cluster a, Cluster b)
        {
            return !(a == b);
        }

        public static bool operator ==(Cluster a, Cluster b)
        {
            return a.Fitness == b.Fitness &&
                a.Tags.Count == b.Tags.Count &&
                a.Tags.All(ta => b.Tags.ContainsKey(ta.Key) && b.Tags[ta.Key] == ta.Value);
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || !(obj is Cluster))
            {
                return false;
            }

            return GetHashCode() == ((Cluster)obj).GetHashCode();
        }

        public override int GetHashCode()
        {
            return Tags.GetHashCode();
        }

        public override string ToString()
        {
            return $"Name '{Name}'\nMessage: '{Message}'\nFitness: '{Fitness}'\nTags: '{string.Join(", ", Tags.Select(t => $"{t.Key}: {t.Value}"))}'";
        }

        #endregion
    }
}
