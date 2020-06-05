using AutotestAnalysis.Models;
using Newtonsoft.Json.Linq;

namespace AutotestAnalysis.Services
{
    public interface IParserManager
    {
        ParsedTestResults ParseTestResults(JArray sessions);
    }
}
