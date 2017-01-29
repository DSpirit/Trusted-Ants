using Assets.Scripts.Manager;
using Assets.Scripts.Norms;
using Assets.Scripts.SettingClasses;
using System;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Settings
{
    public class NormPanel : MonoBehaviour, IPanel
    {
        public Dropdown Norms;

        public Norm CurrentNorm;
        public Toggle Active;
        public InputField Name;
        public Dropdown Type;
        public Dropdown Target;
        public Dropdown Evaluator;
        public Dropdown Condition;
        public Dropdown Content;
        public Dropdown Policies;

        public Policy CurrentPolicy;
        public Dropdown Criterium;
        public Dropdown Comparison;
        public InputField PolicyValue;
        public Dropdown PolicyType;
        public InputField PolicyReward;

        public Setting Setting; 

        public void SetActive(bool active)
        {
            GameObject.FindObjectOfType<SettingsManager>().SetActive("NormPanel");
        }

        public void Start()
        {
            Setting = EnvironmentManager.Instance.Settings;
            InitNormView();
            InitNorm(CurrentNorm);
        }

        public void InitNormView()
        {
            var norms = Setting.Norms;
            Norms.ClearOptions();
            if (norms.Any())
            {
                foreach (var n in norms)
                {
                    Norms.options.Add(new Dropdown.OptionData(n.Name));
                }
                CurrentNorm = norms.First();
            }
            else
            {
                Norms.options.Add(new Dropdown.OptionData("New Norm"));
                CurrentNorm = new Norm();
            }
            Norms.RefreshShownValue();

            Type.ClearOptions();
            foreach (DeonticOperator d in Enum.GetValues(typeof(DeonticOperator)))
            {
                Type.options.Add(new Dropdown.OptionData(d.ToString()));
            }
            Type.RefreshShownValue();

            Target.ClearOptions();
            Evaluator.ClearOptions();
            foreach (Role d in Enum.GetValues(typeof(Role)))
            {
                Target.options.Add(new Dropdown.OptionData(d.ToString()));
                Evaluator.options.Add(new Dropdown.OptionData(d.ToString()));
            }
            Target.RefreshShownValue();
            Evaluator.RefreshShownValue();

            Condition.ClearOptions();
            foreach (Tag d in Enum.GetValues(typeof(Tag)))
            {
                Condition.options.Add(new Dropdown.OptionData(d.ToString()));                
            }
            Condition.RefreshShownValue();

            Content.ClearOptions();
            foreach (Norms.Action d in Enum.GetValues(typeof(Norms.Action)))
            {
                Content.options.Add(new Dropdown.OptionData(d.ToString()));
            }
            Content.RefreshShownValue();

            Criterium.ClearOptions();
            foreach (Option d in Enum.GetValues(typeof(Option)))
            {
                Criterium.options.Add(new Dropdown.OptionData(d.ToString()));
            }
            Criterium.RefreshShownValue();

            Comparison.ClearOptions();
            foreach (Operator d in Enum.GetValues(typeof(Operator)))
            {
                Comparison.options.Add(new Dropdown.OptionData(d.ToString()));
            }
            Comparison.RefreshShownValue();

            PolicyType.ClearOptions();
            foreach (RewardType d in Enum.GetValues(typeof(RewardType)))
            {
                PolicyType.options.Add(new Dropdown.OptionData(d.ToString()));
            }
            PolicyType.RefreshShownValue();
        }
        public void InitNorm(Norm n)
        {
            Active.isOn = n.IsActive;        
            Name.text = n.Name;
            Type.value = (int) n.DeonticOperator;
            Target.value = (int)n.Target;
            Evaluator.value = (int)n.Evaluator;
            Condition.value = (int)n.PertinenceCondition;
            Content.value = (int)n.Content;

            Policies.ClearOptions();
            int i = 1;
            foreach (var po in n.Policies)
            {
                Policies.options.Add(new Dropdown.OptionData(po.ToString()));
                i++;
            }
            
            if (!n.Policies.Any())
                CurrentPolicy = new Policy();
            else
                CurrentPolicy = n.Policies.First();

           InitPolicy(CurrentPolicy);
        }

        public void InitPolicy(Policy p)
        {
            Policies.value = CurrentNorm.Policies.FindIndex(n => n.ToString() == p.ToString());
            Criterium.value = (int)p.Option;
            Comparison.value = (int)p.ComparisonOperator;
            PolicyValue.text = p.Value.ToString("F");
            PolicyType.value = (int)p.RewardType;
            PolicyReward.text = p.Reward.ToString("F");

            Policies.RefreshShownValue();
            Criterium.RefreshShownValue();
            Comparison.RefreshShownValue();
            PolicyType.RefreshShownValue();
        }
        

        public void Update()
        {
            
        }

        public void SelectNorm(int id)
        {
            Norms.value = id;
            CurrentNorm = Setting.Norms.ElementAt(id);
            InitNorm(CurrentNorm);
            Norms.RefreshShownValue();
        }

        public void SelectPolicy(int id)
        {
            Policies.value = id;
            CurrentPolicy = CurrentNorm.Policies.ElementAt(id);
            InitPolicy(CurrentPolicy);
            Policies.RefreshShownValue();
        }

        public void Create()
        {
            CurrentNorm = new Norm {Name = "<New Norm>"};
            Setting.Norms.Add(CurrentNorm);
            InitNorm(CurrentNorm);
            InitNormView();
            SelectNorm(Setting.Norms.Count-1);
            Norms.RefreshShownValue();
        }

        public void Save()
        {
            if (!Setting.Norms.Contains(CurrentNorm))
            {
                Setting.Norms.Add(CurrentNorm);
            }                
            else
            {
                var n = Setting.Norms.Find(m => m.Name == CurrentNorm.Name);
                n = CurrentNorm;
            }
            EnvironmentManager.Instance.Settings = Setting;
        }

        public void AddPolicy()
        {
            var n = new Policy();
            CurrentNorm.Policies.Add(n);
            CurrentPolicy = n;
            
            InitNorm(CurrentNorm);
            InitPolicy(CurrentPolicy);
            SelectPolicy(CurrentNorm.Policies.Count-1);
        }

        public void RemovePolicy()
        {
            CurrentNorm.Policies.Remove(CurrentPolicy);
            InitNorm(CurrentNorm);    
        }

        public void Delete()
        {
            Setting.Norms.Remove(CurrentNorm);
            EnvironmentManager.Instance.Settings = Setting;
            InitNormView();
            InitNorm(Setting.Norms.First());
            SelectNorm(0);
            
        }

        // Norm
        public void SetNormActive(bool active)
        {
            CurrentNorm.IsActive = active;
        }

        public void SetName()
        {
            CurrentNorm.Name = Name.text;
        }

        public void SelectType(int val)
        {
            CurrentNorm.DeonticOperator = (DeonticOperator)val;
        }

        public void SelectTarget(int val)
        {
            CurrentNorm.Target = (Role)val;
        }

        public void SelectEvaluator(int val)
        {
            CurrentNorm.Evaluator = (Role)val;
        }

        public void SelectCondition(int val)
        {
            CurrentNorm.PertinenceCondition = (Tag)val;
        }

        public void SelectContent(int val)
        {
            CurrentNorm.Content = (Scripts.Norms.Action)val;
        }

        // Policy
        public void SelectCriterium(int val)
        {
            CurrentPolicy.Option = (Option)val;
        }

        public void SetRewardValue()
        {
            CurrentPolicy.Reward = float.Parse(PolicyReward.text, NumberStyles.AllowDecimalPoint);
        }

        public void SetPolicyValue()
        {
            CurrentPolicy.Value = float.Parse(PolicyValue.text, NumberStyles.AllowDecimalPoint);
        }

        public void SelectOperator(int val)
        {
            CurrentPolicy.ComparisonOperator = (Operator)val;
        }

        public void SelectRewardType(int val)
        {
            CurrentPolicy.RewardType = (RewardType) val;
        }

        
    }
}
