using System;
using System.Reflection.Emit;
using Assets.Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Development
{
    public class MenuSettings : MonoBehaviour
    {
        public Slider GameSpeed;
        public Text Clock;
        public Text Ticks;
        public Text NewsText;


        public void Start()
        {
            NewsText.text = "0";
        }

        public void Update()
        {
            Clock.text = TimeSpan.FromSeconds(Time.time).ToString().Substring(0,8);
            Ticks.text = string.Format("{0} (in 1M)", TimeSpan.FromSeconds(Time.time).Ticks/1000000);
            int count = EnvironmentManager.Instance.NewsManager.Events.Count;
            NewsText.text =  count < 10 ? count.ToString() : "9+";
        }

        public void SetGameSpeed(float value)
        {
            Time.timeScale = value;
        }
    }
}
