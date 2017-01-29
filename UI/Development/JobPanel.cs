using Assets.Scripts.Manager;
using Assets.Scripts.SettingClasses;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Development
{
    public class JobPanel : MonoBehaviour, IPanel
    {
        public Toggle SpawnJobs;
        public InputField SpawnRate;
        public InputField MinTTL;
        public InputField MaxTTL;
        public InputField SpawnRadius;
        public InputField MinParts;
        public InputField MaxParts;
        public InputField Vanish;
        public InputField HopCount;
        private readonly JobSetting _setting = EnvironmentManager.Instance.Settings.JobSetting;

        public void Start()
        {
            SpawnRate.text = _setting.JobSpawnRate.ToString();
            MinTTL.text = _setting.MinimumTimeToLive.ToString();
            MaxTTL.text = _setting.MaximumTimeToLive.ToString();
            SpawnRadius.text = _setting.JobSpawnRadius.ToString();
            MinParts.text = _setting.MinimumJobParts.ToString();
            MaxParts.text = _setting.MaximumJobParts.ToString();
            Vanish.text = _setting.DeadAliveTime.ToString();
            HopCount.text = _setting.HopCountLimit.ToString();
        }

        public void Update()
        {
            
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void ToggleSpawning(bool spawningEnabled)
        {
            EnvironmentManager.Instance.Controller.ToggleJobSpawning(spawningEnabled);
        }

        public void SetRate(string value)
        {
            _setting.JobSpawnRate = int.Parse(value);
            SaveChanges();
        }

        public void SetMinTTL(string value)
        {
            _setting.MinimumTimeToLive = int.Parse(value);
            SaveChanges();
        }

        public void SetMaxTTL(string value)
        {
            _setting.MaximumTimeToLive = int.Parse(value);
            SaveChanges();
        }

        public void SetRadius(string value)
        {
            _setting.JobSpawnRadius = int.Parse(value);
            SaveChanges();
        }

        public void SetMinParts(string value)
        {
            _setting.MinimumJobParts = int.Parse(value);
            SaveChanges();
        }

        public void SetMaxParts(string value)
        {
            _setting.MaximumJobParts = int.Parse(value);
            SaveChanges();
        }

        public void SetVanish(string value)
        {
            _setting.DeadAliveTime = int.Parse(value);
            SaveChanges();
        }

        public void SetHopCount(string value)
        {
            _setting.HopCountLimit = int.Parse(value);
            SaveChanges();
        }

        private void SaveChanges()
        {
            var setting = EnvironmentManager.Instance.Settings;
            setting.JobSetting = _setting;
            EnvironmentManager.Instance.Settings = setting;
        }

    }
}
