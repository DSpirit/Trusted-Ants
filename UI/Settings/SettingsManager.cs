using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Settings
{
    public class SettingsManager : MonoBehaviour
    {
        public List<Transform> Panels = new List<Transform>();
        public Dropdown Settings;

        public void Start()
        {
            foreach (Transform t in gameObject.transform)
            {
                Panels.Add(t);
            }
                        
        }

        public void SetCurrentSetting(int id)
        {
            string setting = Settings.options[id].text;
            EnvironmentManager.Instance.CurrentSetting = setting;
            Reload();
        }

        public void Reload()
        {
            foreach (var p in Panels)
            {
                if (p.name.Contains("Panel"))
                {
                    p.GetComponent<IPanel>().Start();
                }
            }
        }

        public void SetActive(string name)
        {
            foreach (var p in Panels)
            {
                if (p.name.Contains("Panel"))
                {
                    if (p.name == name)
                    {
                        p.gameObject.SetActive(true);
                    }
                    else
                    {
                        p.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
