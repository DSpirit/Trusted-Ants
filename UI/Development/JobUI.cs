using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Jobs;
using Assets.Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Development
{
    public class JobUI : MonoBehaviour, IPanel
    {
        public Text Jobs;
        public Text JobParts;
        private EnvironmentManager _manager;
        private List<float> SpeedupList = new List<float>(); 
        public float Speedup;

        public void Start()
        {
            _manager = EnvironmentManager.Instance;
        }

        public void Update()
        {
            if (!SpeedupList.Any()) SpeedupList.Add(1);
            Jobs.text = "Current Count: " + _manager.Jobs.Count;
            JobParts.text = "Total: " + _manager.Jobs.Sum(n => n.JobParts.Count);
            JobParts.text += "\n Objects: " + FindObjectsOfType<JobPoint>().Length;
            JobParts.text += "\n Speedup: " + SpeedupList.Average();
        }

        public void AddSpeedup(float speedup)
        {
            SpeedupList.Add(speedup);
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}
