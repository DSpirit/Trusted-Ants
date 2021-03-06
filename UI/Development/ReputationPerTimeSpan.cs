﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets.Scripts.Ant;
using Assets.Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.UI.Development
{
    public class ReputationPerTimeSpan : MonoBehaviour, IPanel
    {
        // Publics
        public WMG_Axis_Graph Graph;
        public List<WMG_Series> Series = new List<WMG_Series>(); 

        // Privates
        private EnvironmentManager _manager;
        private readonly float _repeatRate = 5.0f;

        public void Start()
        {
            _manager = EnvironmentManager.Instance;
            Graph.graphTitleString = "Reputation";
            foreach (AntType type in Enum.GetValues(typeof(AntType)))
            {
                var series = Graph.addSeries();
                Series.Add(series);
                InitSeries(series, type);
            }
            InvokeRepeating("UpdateGraph", 10, _repeatRate);
        }

        public void Update()
        {

        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);  
        }

        private void InitGraph()
        {
            Graph.autoFitLabels = true;
            Graph.xAxis.AxisTitle.GetComponent<Text>().text = "Ticks (in 100K)";
            Graph.xAxis.MaxAutoGrow = true;
            Graph.xAxis.SetLabelsUsingMaxMin = true;
            Graph.yAxis.AxisTitle.GetComponent<Text>().text = "Average Reputation";
        }

        private void InitSeries(WMG_Series series, AntType antType)
        {
            series.seriesName = antType.ToString();
            series.pointColor = GlobalFunctions.AntTypeColorMapping[antType];
            Series.Add(series);
        }

        private void UpdateGraph()
        {
            var ants = EnvironmentManager.Instance.Ants.Select(n => n.Agent);
            foreach (AntType type in Enum.GetValues(typeof(AntType)))
            {
                var typeAnts = ants.Where(n => n.AntType == type);
                var series = Series.Find(n => n.seriesName == type.ToString());
                if (!typeAnts.Any()) continue;
                var value = new Vector2(Time.time, (float) typeAnts.Average(n => n.Rank));
                series.pointValues.Add(value);
            }
        }

    }
}
