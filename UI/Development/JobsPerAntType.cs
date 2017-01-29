using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Ant;
using Assets.Scripts.Manager;
using UnityEngine;

namespace Assets.Scripts.UI.Development
{
    public class JobsPerAntType : MonoBehaviour, IPanel
    {
        public WMG_Axis_Graph JobsPerAntTypeGraph;
        public List<WMG_Series> AntTypeSeries = new List<WMG_Series>();
        public List<string> AgentTypeLabels = new List<string>();
        public WMG_Series FinishedJobsSeries;
        public WMG_Series VerifiedJobsSeries;
        public WMG_Series DistributedJobsSeries;
        public WMG_Series FailedJobsSeries;
        

        // Use this for initialization
        public void Start()
        {
            FinishedJobsSeries = JobsPerAntTypeGraph.addSeries();
            VerifiedJobsSeries = JobsPerAntTypeGraph.addSeries();
            DistributedJobsSeries = JobsPerAntTypeGraph.addSeries();
            FailedJobsSeries = JobsPerAntTypeGraph.addSeries();
            InitSeries(VerifiedJobsSeries, "Verified");
            InitSeries(FinishedJobsSeries, "Finished");
            InitSeries(DistributedJobsSeries, "Distributed");
            InitSeries(FailedJobsSeries, "Failed");
            Init();
            InvokeRepeating("UpdateStats", 10,5);
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        private void InitSeries(WMG_Series series, string type)
        {
            series.AutoUpdateXDistBetween = true;
            series.UseXDistBetweenToSpace = true;
            series.AutoUpdateXDistBetween = true;
            var color = Color.white;
            series.seriesName = type;
            switch (type)
            {
                case "Verified": color = Color.yellow; break;
                case "Finished": color = Color.green; break;
                case "Distributed": color = Color.blue; break;
                case "Failed" : color = Color.magenta; break;
            }
            series.pointColor = color;
        }
       
        public void Update ()
        {
            
        }

        void UpdateStats()
        {
            var a = EnvironmentManager.Instance.Ants.Select(n => n.Agent);
            List<Vector2> failed = new List<Vector2>();
            List<Vector2> verified = new List<Vector2>();
            List<Vector2> distributed = new List<Vector2>();
            List<Vector2> finished = new List<Vector2>();

            foreach (AntType r in Enum.GetValues(typeof(AntType)))
            {
                failed.Add(new Vector2(0, a.Where(n => n.AntType == r).Sum(m => m.Stats.FailedJobs.Count)));
                distributed.Add(new Vector2(0, a.Where(n => n.AntType == r).Sum(m => m.Stats.DistributedJobs.Count)));
                finished.Add(new Vector2(0, a.Where(n => n.AntType == r).Sum(m => m.Stats.FinishedJobs.Count)));
                verified.Add(new Vector2(0, a.Where(n => n.AntType == r).Sum(m => m.Stats.VerifiedJobs.Count)));
            }

            FailedJobsSeries.pointValues.SetList(failed);
            FinishedJobsSeries.pointValues.SetList(finished);
            VerifiedJobsSeries.pointValues.SetList(verified);
            DistributedJobsSeries.pointValues.SetList(distributed);
        }

        void Init()
        {
            foreach (AntType type in Enum.GetValues(typeof(AntType)))
            {
                AgentTypeLabels.Add(type.ToString());
            }
            JobsPerAntTypeGraph.xAxis.AxisNumTicks = Enum.GetValues(typeof(AntType)).Length + 1;
            JobsPerAntTypeGraph.xAxis.axisLabels.SetList(AgentTypeLabels);
            JobsPerAntTypeGraph.yAxis.MaxAutoGrow = true;
        }
    }
}
