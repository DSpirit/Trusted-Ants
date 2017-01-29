using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Assets.Scripts.Ant;
using Assets.Scripts.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Development
{
    
    public class SpeedupPerTimeSpan : MonoBehaviour, IPanel
    {
        // Publics
        public WMG_Axis_Graph Graph;
        public List<WMG_Series> Series = new List<WMG_Series>();
        public AntStatsObserver Observer;
        public string Category = "CUN";

        // Privates
        private EnvironmentManager _manager;
        private float _repeatRate = 60.0f;
        private float _restart = 100;

        public void Start()
        {
            _manager = EnvironmentManager.Instance;
            Graph.graphTitleString = "Speedup";
            foreach (AntType type in Enum.GetValues(typeof(AntType)))
            {
                var series = Graph.addSeries();
                Series.Add(series);
                InitSeries(series, type);
            }
            Observer = FindObjectOfType<AntStatsObserver>();
            InvokeRepeating("UpdateGraph", 10, _repeatRate);
        }

        public void Update()
        {

        }

        public void SaveData()
        {
            string directory = GlobalFunctions.CreateDirectory(Category);
            foreach (var series in Series.Where(m => EnvironmentManager.Instance.Ants.Any(n => n.Agent.AntType.ToString() == m.seriesName)))
            {
                string fileName = "speedup.csv";
                string pathString = Path.Combine(directory, fileName);
                using (TextWriter tw = new StreamWriter(pathString, true))
                {
                    string dataset;
                    foreach (var s in series.pointValues)
                    {
                        dataset = string.Format("{0:#}\t{1}\t{2}", s.x, s.y.ToString(new CultureInfo("de-DE")), series.seriesName);
                        tw.WriteLine(dataset);
                    }
                    tw.Close();
                }
            }
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);  
        }

        private void InitGraph()
        {
            Graph.autoFitLabels = true;
            Graph.xAxis.AxisTitle.GetComponent<Text>().text = "Seconds";
            Graph.xAxis.MaxAutoGrow = true;
            Graph.xAxis.SetLabelsUsingMaxMin = true;
            Graph.yAxis.AxisTitle.GetComponent<Text>().text = "Average Speedup";
        }

        private void InitSeries(WMG_Series series, AntType antType)
        {
            series.seriesName = antType.ToString();
            series.pointColor = GlobalFunctions.AntTypeColorMapping[antType];
            Series.Add(series);
        }

        private void UpdateGraph()
        {
            var ants = _manager.Ants.Select(n => n.Agent);
            foreach (AntType type in Enum.GetValues(typeof(AntType)))
            {
                var typeAnts = ants.Where(n => n.AntType == type);
                var series = Series.Find(n => n.seriesName == type.ToString());
                if (typeAnts.Any())
                {
                    float avg = Observer.Speedup[type].Any() ? Observer.Speedup[type].Average() : 0;
                    if (avg.Equals(0))
                        continue;
                    series.pointValues.Add(new Vector2(Time.timeSinceLevelLoad, avg));
                    Observer.Speedup[type].Clear();
                }
                    
            }
            
        }

    }
}
