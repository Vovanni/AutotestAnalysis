using AutotestAnalysis.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutotestAnalysis.Services
{
    public interface IParserManager
    {
        ParsedTestResults ParseTestResults(JArray sessions);
    }
}
