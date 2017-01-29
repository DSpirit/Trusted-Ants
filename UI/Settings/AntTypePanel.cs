using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Ant;
using Assets.Scripts.Manager;
using Assets.Scripts.SettingClasses;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Settings
{
    public class AntTypePanel : MonoBehaviour, IPanel
    {
        public Setting GeneralSettings = new Setting();
        // Input Fields Ant Settings
        public Text JobAcceptationValue;
        public Text ReputationRelevanceValue;

        // Slider Ant Settings
        public Slider TurquoiseSlider;
        public Slider BlueSlider;
        public Slider PurpleSlider;
        public Slider GreenSlider;
        public Slider TurquoiseSliderRnd;
        public Slider BlueSliderRnd;
        public Slider PurpleSliderRnd;
        public Slider GreenSliderRnd;
        public Slider JobAcceptationSlider;
        public Slider ReputationRelevanceSlider;
        public List<Slider> SpawnCountSlidersFixed = new List<Slider>();
        public List<Slider> SpawnCountSlidersRandom = new List<Slider>();

        // Detail Fields (Distribution)
        public InputField TurquoiseFixed;
        public InputField BlueFixed;
        public InputField PurpleFixed;
        public InputField GreenFixed;
        public List<InputField> FixedCountInputFields = new List<InputField>();

        // Random Detail Fields
        public InputField MaxTurquoiseRandomCount;
        public InputField MaxBlueRandomCount;
        public InputField MaxPurpleRandomCount;
        public InputField MaxGreenRandomCount;
        public List<InputField> RandomCountInputFields = new List<InputField>();

        // Dropdown
        public Dropdown AntTypeDropdown;
        public Dropdown JobSortOrderDropdown;
        public Dropdown BehaviourTypeDropdown;

        // Toggles
        public Toggle FinishJobs;
        public Toggle AcceptFromWellBehaving;
        public Toggle AcceptFromBadBehaving;

        public AntType CurrentAntType;

        public void Start()
        {
            // TODO: Update to current settings!
            InitDefaultSettings();
            GetGeneralSettings();
            GetAntSettings();
            InitUISettings();
            Seed();
        }

        public void InitDefaultSettings()
        {
            Setting set = EnvironmentManager.Instance.Settings;
            if (set == null) return;
            GeneralSettings = set;
        }

        public void GetGeneralSettings()
        {

        }

        public void GetAntSettings()
        {
            // Init Lists
            FixedCountInputFields.AddRange(new List<InputField>() { PurpleFixed, BlueFixed, GreenFixed, TurquoiseFixed });
            RandomCountInputFields.AddRange(new List<InputField>() { MaxBlueRandomCount, MaxGreenRandomCount, MaxPurpleRandomCount, MaxTurquoiseRandomCount });
            SpawnCountSlidersFixed.AddRange(new List<Slider>() { TurquoiseSlider, BlueSlider, PurpleSlider, GreenSlider });
            SpawnCountSlidersRandom.AddRange(new List<Slider>() { TurquoiseSliderRnd, BlueSliderRnd, PurpleSliderRnd, GreenSliderRnd });
        }

        public void reset()
        {
            Seed();
        }


        public void InitUISettings()
        {
            Debug.Log("Loading Settings...");

            // Ant Settings
            foreach (var role in Enum.GetValues(typeof(AntType)))
            {
                AntTypeDropdown.options.Add(new Dropdown.OptionData(role.ToString()));
            }

            foreach (var so in Enum.GetValues(typeof(SortOrder)))
            {
                JobSortOrderDropdown.options.Add(new Dropdown.OptionData(so.ToString()));
            }

            foreach (var b in Enum.GetValues(typeof(AntTypeBehaviour)))
            {
                BehaviourTypeDropdown.options.Add(new Dropdown.OptionData(b.ToString()));
            }

            AntTypeDropdown.captionText.text = AntTypeDropdown.options[0].text;
            JobSortOrderDropdown.captionText.text = JobSortOrderDropdown.options[0].text;
            BehaviourTypeDropdown.captionText.text = BehaviourTypeDropdown.options[0].text;

            Debug.Log("Finished Loading!");
        }


        public void Seed()
        {
            // Get first Ant Type to display current values on GUI
            CurrentAntType = (AntType)Enum.GetValues(typeof(AntType)).GetValue(0);
            AntTypeDropdown.value = (int)CurrentAntType;

            // Get Settings from first Ant Type
            var setting = GeneralSettings.AntTypeSettings.FirstOrDefault(n => n.AntType == CurrentAntType);

            SetDistributionGUIValues(setting.DistributionSetting);
            SetBehaviourGUIValues(setting.BehaviourSetting);
        }
        
        public void UpdateAntTypeSettings()
        {
            AntTypeSetting s = GeneralSettings.AntTypeSettings.Find(n => n.AntType == CurrentAntType);
            s.DistributionSetting.FixedCount.Clear();
            s.DistributionSetting.RandomCount.Clear();
            foreach (Groups g in Enum.GetValues(typeof(Groups)))
            {
                var newFixed = new AntDistributionSetting(g, int.Parse(FixedCountInputFields.Find(n => n.name.Contains(g.ToString())).text));
                var newRandom = new AntDistributionSetting(g, int.Parse(RandomCountInputFields.Find(n => n.name.Contains(g.ToString())).text));
                s.DistributionSetting.FixedCount.Add(newFixed);
                s.DistributionSetting.RandomCount.Add(newRandom);
            }

            SetDistributionGUIValues(s.DistributionSetting);
            s.BehaviourSetting.Behaviour = (AntTypeBehaviour)BehaviourTypeDropdown.value;
            s.BehaviourSetting.JobAcceptationRateMax = JobAcceptationSlider.value / 100;
            s.BehaviourSetting.JobTtlSortOrder = (SortOrder)JobSortOrderDropdown.value;
            s.BehaviourSetting.ReputationRelevanceMax = ReputationRelevanceSlider.value / 100;
            s.BehaviourSetting.WillFinishJob = FinishJobs.isOn;
            SetBehaviourGUIValues(s.BehaviourSetting);
        }

        public void SelectSortOrder(int val)
        {
            var setting = GeneralSettings.AntTypeSettings.Find(n => n.AntType == CurrentAntType);
            setting.BehaviourSetting.JobTtlSortOrder = (SortOrder)val;

        }

        public void SelectType(int val)
        {
            var setting = GeneralSettings.AntTypeSettings.Find(n => n.AntType == CurrentAntType);
            setting.BehaviourSetting.Behaviour = (AntTypeBehaviour)val;
        }

        private void SetBehaviourGUIValues(BehaviourSetting bSetting)
        {
            JobSortOrderDropdown.value = (int)bSetting.JobTtlSortOrder;
            JobAcceptationSlider.value = bSetting.JobAcceptationRateMax * 100;
            JobAcceptationValue.text = bSetting.JobAcceptationRateMax.ToString("p");
            FinishJobs.isOn = bSetting.WillFinishJob;
            ReputationRelevanceSlider.value = bSetting.ReputationRelevanceMax * 100;
            ReputationRelevanceValue.text = bSetting.ReputationRelevanceMax.ToString("p");
            BehaviourTypeDropdown.value = (int)bSetting.Behaviour;
            AcceptFromBadBehaving.isOn = bSetting.AcceptsAntTypeBehaviours.Contains(AntTypeBehaviour.BadBehaving);
            AcceptFromWellBehaving.isOn = bSetting.AcceptsAntTypeBehaviours.Contains(AntTypeBehaviour.WellBehaving);
        }

        // Events
        // Ant Type
        public void SaveAntTypeSettings()
        {
            try
            {
                UpdateAntTypeSettings();
                EnvironmentManager.Instance.Settings = GeneralSettings;
                Debug.Log("Settings saved!");
            }
            catch
            {
                Debug.Log("Format Exception!");
            }
        }
        public void ResetAntTypeSettings()
        {
            GeneralSettings.InitAntTypeSettings();
            InitUISettings();
        }

        public void SetCurrentAntType(int val)
        {
            SaveAntTypeSettings();
            CurrentAntType = (AntType)val;
            // Update GUI Values
            AntTypeSetting currentAntTypeSetting = GeneralSettings.AntTypeSettings.FirstOrDefault(n => n.AntType == CurrentAntType);
            DistributionSetting distribution = currentAntTypeSetting.DistributionSetting;
            BehaviourSetting behaviour = currentAntTypeSetting.BehaviourSetting;

            SetDistributionGUIValues(distribution);
            SetBehaviourGUIValues(behaviour);
        }

        public void TglAcceptFromWellBehaving(bool val)
        {
            AntTypeSetting currentAntTypeSetting = GeneralSettings.AntTypeSettings.Find(n => n.AntType == CurrentAntType);
            if (currentAntTypeSetting == null) return;

            if (val)
                currentAntTypeSetting.BehaviourSetting.AcceptsAntTypeBehaviours.Add(AntTypeBehaviour.WellBehaving);
            else
            {
                currentAntTypeSetting.BehaviourSetting.AcceptsAntTypeBehaviours.Remove(AntTypeBehaviour.WellBehaving);
            }
        }

        public void TglAcceptFromBadBehaving(bool val)
        {
            AntTypeSetting currentAntTypeSetting = GeneralSettings.AntTypeSettings.Find(n => n.AntType == CurrentAntType);
            if (currentAntTypeSetting == null) return;

            if (val)
                currentAntTypeSetting.BehaviourSetting.AcceptsAntTypeBehaviours.Add(AntTypeBehaviour.BadBehaving);
            else
            {
                currentAntTypeSetting.BehaviourSetting.AcceptsAntTypeBehaviours.Remove(AntTypeBehaviour.BadBehaving);
            }
        }

        private void SetDistributionGUIValues(DistributionSetting s)
        {
            foreach (Groups g in Enum.GetValues(typeof(Groups)))
            {
                FixedCountInputFields.Find(n => n.name.Contains(g.ToString())).text = s.FixedCount.Find(n => n.AntGroup == g).Count.ToString();
                RandomCountInputFields.Find(n => n.name.Contains(g.ToString())).text = s.RandomCount.Find(n => n.AntGroup == g).Count.ToString();
                SpawnCountSlidersFixed.Find(n => n.name.Contains(g.ToString())).value = s.FixedCount.Find(n => n.AntGroup == g).Count;
                SpawnCountSlidersRandom.Find(n => n.name.Contains(g.ToString())).value = s.RandomCount.Find(n => n.AntGroup == g).Count;
            }
        }

        // OnSliderChangeEvents
        public void UpdateInput(string group)
        {
            UpdateDistributionGUI(group);
        }

        public void UpdateTextValue(GameObject name)
        {
            int a = 4;
        }

        public void UpdateDistributionGUI(string group)
        {
            switch (group)
            {
                case "blue":
                    BlueFixed.text = BlueSlider.value.ToString();
                    break;
                case "green":
                    GreenFixed.text = GreenSlider.value.ToString();
                    break;
                case "purple":
                    PurpleFixed.text = PurpleSlider.value.ToString();
                    break;
                case "turquoise":
                    TurquoiseFixed.text = TurquoiseSlider.value.ToString();
                    break;
                case "blueRnd":
                    MaxBlueRandomCount.text = BlueSliderRnd.value.ToString();
                    break;
                case "greenRnd":
                    MaxGreenRandomCount.text = GreenSliderRnd.value.ToString();
                    break;
                case "purpleRnd":
                    MaxPurpleRandomCount.text = PurpleSliderRnd.value.ToString();
                    break;
                case "turquoiseRnd":
                    MaxTurquoiseRandomCount.text = TurquoiseSliderRnd.value.ToString();
                    break;
                case "jobAcceptationRate":
                    JobAcceptationValue.text = JobAcceptationSlider.value.ToString();
                    break;
                case "reputationRelevance":
                    ReputationRelevanceValue.text = ReputationRelevanceSlider.value.ToString();
                    break;
            }
        }

        public int GetTotalAntsFromDetails()
        {
            int sumTurquoise = int.Parse(TurquoiseFixed.text);
            int sumBlue = int.Parse(BlueFixed.text);
            int sumPurple = int.Parse(PurpleFixed.text);
            int sumGreen = int.Parse(GreenFixed.text);

            return sumGreen + sumTurquoise + sumBlue + sumPurple;
        }

        public void Update()
        {
            
        }

        public void SetActive(bool active)
        {
            GameObject.FindObjectOfType<SettingsManager>().SetActive("AntTypePanel");
        }
    }
}
