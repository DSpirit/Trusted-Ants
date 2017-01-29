using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Development
{
    public class StatsPanel : MonoBehaviour, IPanel
    {
        public Toggle JobToggle;
        public Toggle AntToggle;
        public GameObject AntStats;
        public GameObject JobStats;
        public void Start()
        {
            JobToggle.isOn = JobStats.gameObject.activeSelf;
            AntToggle.isOn = AntStats.gameObject.activeSelf;
        }

        public void Update()
        {
            
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void ToggleAnts(bool active)
        {
            AntStats.gameObject.SetActive(active);
        }

        public void ToggleJobs(bool active)
        {
            JobStats.gameObject.SetActive(active);
        }
    }
}
