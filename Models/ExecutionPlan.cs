using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace test.Models
{
    public class ExecutionPlan
    {
        public Plan Plan { set; get; }

        [JsonProperty("Planning Time")]
        public decimal PlanningTime { set; get; }

        [JsonProperty("Execution Time")]
        public decimal ExecutionTime { set; get; }
    }

    public class Plan
    {
        [JsonProperty("Node Type")]
        public string NodeType { set; get; }

        [JsonProperty("Parallel Aware")]
        public bool ParallelAware { set; get; }

        [JsonProperty("Relation Name")]
        public string RelationName{ set; get; }

        public string Alias { set; get; }

        [JsonProperty("Startup Cost")]
        public decimal StartupCost { set; get; }

        [JsonProperty("Total Cost")]
        public decimal TotalCost { set; get; }

        [JsonProperty("Plan Rows")]
        public int PlanRows { set; get; }

        [JsonProperty("Plan Width")]
        public int PlanWidth { set; get; }

        [JsonProperty("Actual Startup Time")]
        public decimal ActualStartupTime { set; get; }

        [JsonProperty("Actual Total Time")]
        public decimal ActualTotalTime { set; get; }

        [JsonProperty("Actual Rows")]
        public int ActualRows { set; get; }

        [JsonProperty("Actual Loops")]
        public int ActualLoops { set; get; }
    }
}
