using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Development
{
    public class NewsPanel : MonoBehaviour, IPanel
    {
        public Text AntText;
        public Text ActionText;
        public Text PartnerText;
        public Text DescriptionText;
        public Text TimeText;
        public RawImage Star;
        private NewsManager _news;
        

        public void Start()
        {
            ClearNews();
            _news = EnvironmentManager.Instance.NewsManager;
            InvokeRepeating("UpdateNews", 10, 5);
        }

        public void Update()
        {
            if (_news.Events.Any())
            {
                Star.color = Color.yellow;
            }
            else
            {
                Star.color = Color.white;
            }
        }

        public void UpdateNews()
        {
            ClearNews();
            foreach (var @event in _news.Events)
            {
                AntText.text += @event.Ant + "\n";
                ActionText.text += @event.Action + "\n";
                PartnerText.text += @event.Partner + "\n";
                DescriptionText.text += @event.Description + "\n";
                TimeText.text += @event.Time + "\n";
            }
        }

        public void ClearNews()
        {
            AntText.text = "";
            ActionText.text = "";
            PartnerText.text = "";
            DescriptionText.text = "";
            TimeText.text = "";
        }

        public void SetActive(bool active)
        {
            if (!active)
                EnvironmentManager.Instance.NewsManager.Events.Clear();
            gameObject.SetActive(active);
        }
    }
}
