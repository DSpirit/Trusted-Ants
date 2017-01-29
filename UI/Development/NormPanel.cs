using System;
using System.Linq;
using Assets.Scripts.Manager;
using Assets.Scripts.Norms;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Development
{
    public class NormPanel : MonoBehaviour, IPanel
    {
        public Dropdown Norms;
        public Toggle ToggleNorm;
        public Norm CurrentNorm;
        public NormManager Manager;

        public void Start()
        {
            Manager = EnvironmentManager.Instance.NormManager;
            foreach (var norm in Manager.Norms)
            {
                Norms.options.Add(new Dropdown.OptionData(norm.Name));
            }
            Norms.options[0].text = Norms.options[0].text;
            SetCurrentNorm(0);
        }

        public void Update()
        {
            
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void SetCurrentNorm(int index)
        {
            try
            {
                var name = Norms.options[index].text;
                var norm = Manager.Norms.Find(n => n.Name == name);
                CurrentNorm = norm;
                ToggleNorm.isOn = norm.IsActive;
            }
            catch { }
            
        }

        public void NormToggle(bool value)
        {
            CurrentNorm.IsActive = value;
        }
    }
}
