﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Ant;
using Assets.Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Development
{
    public class AverageSpeedup : MonoBehaviour, IPanel
    {
        // Publics
        public WMG_Axis_Graph Graph;
        public WMG_Series Series;
        public List<Vector2> SpeedupList = new List<Vector2>();
        public List<string> AgentTypeLabels = new List<string>();
        public AntStatsObserver Observer;

        // Privates
        private EnvironmentManager _manager;
        public void Start()
        {
            _manager = EnvironmentManager.Instance;
            Graph.graphTitleString = "Average Speedup by Ant Type";
            Series = Graph.addSeries();
            Init();
            InvokeRepeating("GetData", 10, 10.0f);
            Observer = FindObjectOfType<AntStatsObserver>();
        }

        public void Update()
        {
            
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void GetData()
        {
            var ants = _manager.Ants.Select(n => n.Agent);
            SpeedupList.Clear();
            SpeedupList.Add(new Vector2(1, 0));
            int num = 2;
            foreach (AntType role in Enum.GetValues(typeof(AntType)))
            {
                var antType = role;
                var antsPerType = ants.Where(n => n.AntType == antType && n.TrustRatings.Any());
                if (antsPerType.Any())
                {
                    float speedup = Observer.Speedup[role].Average();
                    SpeedupList.Add(new Vector2(num, speedup));
                }
                SpeedupList.Add(new Vector2(num, 0));
                num++;
            }
            
            Series.pointValues.SetList(SpeedupList);
        }

        private void Init()
        {
            AgentTypeLabels.Add("");
            Graph.groups.Add("");
            Series.pointColors.Add(Color.clear);
            
            foreach (AntType type in Enum.GetValues(typeof(AntType)))
            {
                Graph.groups.Add(type.ToString());
                Series.pointColors.Add(GlobalFunctions.AntTypeColorMapping[type]);
            }
            Graph.useGroups = true;
            Graph.xAxis.LabelType = WMG_Axis.labelTypes.groups;
            Graph.yAxis.MaxAutoGrow = true;
            Graph.xAxis.AxisNumTicks = Graph.groups.Count;
            Graph.xAxis.axisLabels.SetList(AgentTypeLabels);
            Graph.yAxis.SetLabelsUsingMaxMin = true;
            Graph.autoFitLabels = true;
            Series.usePointColors = true;
            Series.AutoUpdateXDistBetween = true;
            Series.UseXDistBetweenToSpace = true;
            Series.AutoUpdateXDistBetween = true;
        }
    }
}
