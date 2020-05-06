using AutotestAnalysis.Models;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AutotestAnalysis.Services
{
    public class ParserManager : IParserManager
	{

		private string[] _exclusions = new[]
			{
				"the",
				"in",
				"was",
				"could",
				"not",
				"is",
				"for",
				"that",
				"with",
				"no",
				"this",
				"of",
				"to",
				"an",
				"a",
				"if",
				"didnt",
				"and",
				"couldnt",
				"cannot",
				"any",
				"got",
				"from",
				"have",
				"are",
				"too",
				"there",
				"doesnt",
				"by",
				"but",
				"each",
				"it",
				"more",
				"-",
				"than",
				"test",
				"step",
				"set",
				"on",
				"now",
				"new",
				"must",
				"be",
				"cant",
				"or",
				"same",
				"at",
				"after",
				"via",
				"made",
				"all",
				"as",
				"beacause",
				"found"
			};

		public ParserManager()
		{
			//ParseTestResults(@"D:\User\Desktop\response_1588674070715.json");
		}

        public ParsedTestResults ParseTestResults (JArray sessions)
        {
			var clusters = new List<Cluster>();

			var i = 0;
			var keys = new List<string>();
			var keysDict = new Dictionary<int, string>();
			//var sessions = JArray.Parse(File.ReadAllText(jsonPath));

			//var indexedMessages = new List<Dictionary<int, int>>();

			foreach (var session in sessions)
			{
				var product = session["description"]["product"].ToString()+session["description"]["version"].ToString().Split('.').First();
				foreach (var testResult in session["testResults"])
				{
					var name = testResult["name"].ToString();
					foreach (var method in testResult["methods"])
					{
						foreach (var attempt in method["attempts"])
						{
							var message = attempt["message"].ToString();
							if (string.IsNullOrWhiteSpace(message) || message.Contains("DeployWithDeployer"))
							{
								continue;
							}

							var parsed = ParseMessage(message);

							var indexedMessage = ParseKeys(ref keys, parsed);

							i++;

							var cluster = new Cluster(
								product: product,
								name: name, 
								platform: attempt["platform"].ToString(), 
								message: message,
								tags: indexedMessage);;

							var equalCluster = clusters.FirstOrDefault(c => c == cluster);

							if (equalCluster is null)
							{
								clusters.Add(cluster);
								//Log.Debug("Add new cluster: {cluster}", cluster);
							}
							else
							{
								clusters.Remove(equalCluster);
								var mergedCluster = new Cluster(new List<Cluster> { cluster, equalCluster });
								//Log.Debug("Merge new cluster: {cluster}", mergedCluster);
							}
						}
					}
				}
			}

			//Исключаем все одиночные вхождения
			for (int k = 0; k < keys.Count; k++)
			{
				var count = 0;
				foreach (var cluster in clusters)
				{
					if (cluster.Tags.ContainsKey(k))
					{
						count++;
						if (count > 1)
						{
							break;
						}
					}
				}

				if (count == 1)
				{
					var cluster = clusters.First(d => d.Tags.ContainsKey(k));
					cluster.Tags.Remove(k);
					Log.Debug("Remove key: {key} with id: {id}", keys[k], k);
				}
				else
				{
					keysDict.Add(k, keys[k]);
				}
			}

			Log.Debug("Updated keys: {keys}", keysDict);
			Log.Debug("Clusters count: {count}, failed tests count: {all}", clusters.Count, i);
			Log.Debug("Prepared clusters: {clusters}", string.Join("\n", string.Join("\n-----------------------\n", clusters)));

			return new ParsedTestResults { Keys = keysDict, Clusters = clusters };
		}

		private Dictionary<int,int> ParseKeys(ref List<string> keys, string[] message)
		{
			var output = new Dictionary<int, int>(); 

			foreach (var word in message)
			{
				if (_exclusions.Any(e => e.Equals(word, StringComparison.InvariantCultureIgnoreCase)))
				{
					continue;
				}
				
				if(GetId(keys, word, out var id))
				{
					if (output.ContainsKey(id))
					{
						output[id] = output[id] + 1;
						continue;
					}

					output.Add(id, 1);
					continue;
				}

				keys.Add(word);
				output.Add(id, 1);
			}

			return output;
		}

		private bool GetId(List<string> keys, string word, out int id)
		{
			for (int i = 0; i < keys.Count(); i++)
			{
				if (keys[i].Equals(word, StringComparison.InvariantCultureIgnoreCase))
				{
					id = i;
					return true;
				}
			}

			id = keys.Count;
			return false;
		}

        private string[] ParseMessage(string message)
        {
			
			var replacedString = Regex.Replace(message, @"MultiCheckAssert\s+failed.\s+|\*|\[|\]|\(|\)|\{|\}|:|=|\'|\!|,", "").ToLower();
            var replacedString2 = Regex.Replace(replacedString, @"passed.*\n|failed|\#.*\d+\s+step\s+(\d|\.)+\s|\#\d+|\n|\\|/|\.|\?", " ").Replace("\"", "");
			var replacedString3 = Regex.Replace(replacedString2, @"\s+", " ").Trim();
            return replacedString3.Split(' ');
        }
    }
}
