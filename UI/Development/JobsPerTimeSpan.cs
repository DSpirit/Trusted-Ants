using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Development
{
    public class JobsPerTimeSpan : MonoBehaviour, IPanel
    {
        public Statistics Statistics;
        public static int StatisticsRate = 5;
        public float StatisticsStart = 0;

        public WMG_Axis_Graph Graph;
        public WMG_Series FinishedJobPartsPerRate;
        public WMG_Series FinishedJobsPerRate;
        public WMG_Series FailedJobsPerRate;

        public void Start()
        {
            Statistics = Statistics.Instance();
            GameObject.Find("Legend-Standard").GetComponent<WMG_Legend>().numRowsOrColumns = 3;
            StatisticsRate = 5;
            InvokeRepeating("UpdateStatistics", 0.0f, StatisticsRate);
            GameObject.Find("MyGraphTitle").GetComponent<Text>().text = "Graph updated every " + StatisticsRate + " seconds";
        }

        public void Update()
        {
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void UpdateStatistics()
        {
            // Jobs
            var finishedJobPartsPerTimeSpan = Statistics.FinishedJobPartsPerTimeSpan(TimeSpan.FromSeconds(StatisticsRate), StatisticsStart);
            finishedJobPartsPerTimeSpan.Sort((a, b) => a.x.CompareTo(b.x));
            FinishedJobPartsPerRate.pointValues.SetList(finishedJobPartsPerTimeSpan);
            //WMG_Axis_Graph graph = graphGO.GetComponent<WMG_Axis_Graph> ();
            var list2 = Statistics.FinishedJobsPerTimeSpan(TimeSpan.FromSeconds(StatisticsRate), StatisticsStart);
            list2.Sort((a, b) => a.x.CompareTo(b.x));
            FinishedJobsPerRate.pointValues.SetList(list2);

            var list3 = Statistics.FailedJobsPerTimeSpan(TimeSpan.FromSeconds(StatisticsRate), StatisticsStart);
            list3.Sort((a, b) => a.x.CompareTo(b.x));
            FailedJobsPerRate.pointValues.SetList(list3);
        }

        public void ResetGraph()
        {
            StatisticsStart = Time.time;
            var list = new List<Vector2>();
            list.Add(new Vector2(0, 0));
            FinishedJobPartsPerRate.pointValues.SetList(list);
            FinishedJobsPerRate.pointValues.SetList(list);
            FailedJobsPerRate.pointValues.SetList(list);
            Graph.xAxis.AxisMaxValue = 20;
        }
    }
}
