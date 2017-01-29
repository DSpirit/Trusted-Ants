using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Assets.Scripts.Ant.AgentTypes;
using Assets.Scripts.Manager;
using Assets.Scripts.Norms;
using UnityEngine;
using UnityEngine.UI;
using GameManager = FlipWebApps.GameFramework.Scripts.GameStructure.GameManager;

namespace Assets.Scripts.Player
{
    [Serializable]
    public class Task : MonoBehaviour
    {
        public Agent Target;
        public Policy Policy;
        public float TTL;
        public float TotalTime;
        public bool Finished;
        public GameObject[] Obstacles;
        public bool Activated;
        public int Hits;
        public float ParticleHits;

        // UI
        public List<Texture> Textures = new List<Texture>();
        public RawImage Image;
        public Text TTLText;
        public Text Description;
        public Text HitsText;

        public Vector3 TaskPosition = new Vector3(0, 0);

        private Color _c = new Color();
        private FlowManager _flow;



        public void Start()
        {
            _flow = EnvironmentManager.Instance.FlowManager;
            _flow.InitializeTask(this);
            Init();
            InvokeRepeating("CheckState", 0, 1);
        }

        public void Init()
        {
            TotalTime = TTL;
            float alpha = Finished ? 0 : 1;
            GetComponent<Image>().CrossFadeAlpha(alpha, 0, true);
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(!Finished);
            }
            string targetColor = Policy.RewardType == RewardType.Incentive ? "#52FF00C7" : "#FF1600C7";
            ColorUtility.TryParseHtmlString(targetColor, out _c);
            GetComponent<Image>().color = _c;
            Image.texture = Textures.ElementAt((int)Target.AntType);
            TimeSpan time = TimeSpan.FromSeconds(TTL);
            TTLText.text = string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
            Description.text = Policy.ToString();
            
            Activated = true;

            // Set Arrow Color
            Target.Ant.GetComponentInChildren<Pointer>(true).GetComponentInChildren<MeshRenderer>(true).material.color = _c;
        }

        public void Update()
        {
            // Set Arrow Color
            Target.Ant.GetComponentInChildren<Pointer>(true).GetComponentInChildren<MeshRenderer>(true).material.color = _c;
            if (Activated && !Finished)
            {
                TTL -= Time.deltaTime;
                CheckState();
                TimeSpan time = TimeSpan.FromSeconds(TTL);
                TTLText.text = string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
                HitsText.text = string.Format("{0}/{1}", Hits, Policy.Reward);
                if (Hits >= Policy.Reward)
                {
                    Target.Ant.Ani.SetTrigger("FinishedFighting");
                    Finished = true;
                    transform.SetParent(EnvironmentManager.Instance.Player.Tasks);
                    SetScore();
                }
            }
            if (Finished)
            {
                try
                {
                    Target.Ant.GetComponentInChildren<Pointer>().gameObject.SetActive(false);
                }
                catch { }
            }
        }

        private void SetScore()
        {
            var player = EnvironmentManager.Instance.Player;
            player.Stats.MissionsDone++;
            int scoreFromTime = (int)(TotalTime - TTL);
            player.IncreaseScore(scoreFromTime);
            _flow.MissionsFulfilled++;
            GameManager.Instance.Player.Coins++;
        }

        private void CheckState()
        {
            if (TTL <= 0)
            {
                Target.Ant.Ani.SetTrigger("FinishedFighting");
                Finished = true;
            }
        }
    }




}
