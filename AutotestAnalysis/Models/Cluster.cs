using Microsoft.AspNetCore.Razor.TagHelpers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private string _name;

        public string Name => _name ?? string.Join("\n", Childs.Select(c => c.Name.ToString()));

        private string _message;
        public string Message => _message ?? string.Join("\n", Childs.Select(c => c.Message.ToString()));

        public Cluster(string product, string name, string platform, string message, Dictionary<int, int> tags)
        {
            _name = $"{product}-{name}-{platform}";
            _message = message;//$"{product}-{platform}\n{message}";
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

            //Считаем сумму
            foreach (var cluster in clustersToMerge)
            {
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
                    Fitness += Math.Abs(tag.Value - (cluster.Tags.ContainsKey(tag.Key) ? cluster.Tags[tag.Key] : 0)) / Tags.Count;
                }
            }
        }

        public static float ComputeFitness(List<Cluster> clusters)
        {
            return new Cluster(clusters).Fitness;
        }

        public Dendrogram GetDendrogram()
        {
            var dendrogram = new Dendrogram { Name = "" };

            if (Childs == null || !Childs.Any())
            {
                dendrogram.Name = Name;
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
