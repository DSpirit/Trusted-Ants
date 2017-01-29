using System.Collections.Generic;
using Assets.Scripts.Ant.AgentTypes;
using Assets.Scripts.Norms;
using Assets.Scripts.Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Manager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        public TaskPanel TaskPanel;

        public PlayerPanel HUD;
        public GameObject TaskPrefab;
        [SerializeField]
        public List<Task> Tasks = new List<Task>();
        public CanvasGroup MyCg;
        public bool Flash = false;
        public AntEater Player;
        public float SpawnRadius = 300f;
        public FlowManager FlowManager;

        // Powerups
        public GameObject Container;
        public List<GameObject> Powerups = new List<GameObject>();

        public GameObject Healthpack;
        public GameObject Manapack;
        public GameObject Speedup;

        // Use this for initialization
        void Start()
        {
            FlowManager = EnvironmentManager.Instance.FlowManager;
            InitPowerups();
            InvokeRepeating("SpawnPowerUps", 0, 60);
            foreach (var instanceAnt in EnvironmentManager.Instance.Ants)
            {
                instanceAnt.StateBubble.BubbleImageComponent.enabled = false;
            }
        }

        void InitPowerups()
        {
            Powerups.Add(Healthpack);
            Powerups.Add(Manapack);
            Powerups.Add(Speedup);
        }

        // Update is called once per frame
        void Update()
        {
            if (Player.gameObject.activeSelf && !Player.Items.gameObject.activeSelf)
            {
                Player.Items.gameObject.SetActive(true);
                HUD.gameObject.SetActive(true);
            }
            FadeDisplay();
        }

        public void SpawnPowerUps()
        {
            if (!FlowManager) return;
            int flowAffection = 10 - (int) (FlowManager.FlowLevel*10);
            foreach (GameObject powerup in Powerups)
            {
                for (int i = 0; i < Random.Range(0, flowAffection); i++)
                {
                    var obj = (GameObject)Instantiate(powerup, GlobalFunctions.RandomPointOnNavmesh(SpawnRadius), Quaternion.identity);
                    obj.transform.SetParent(Container.transform);
                }
            }
        }

        public void AddTask(Agent target, Policy policy)
        {
            // Distinguish between Game Mode and Developer Mode
            // # Automated Norm Execution
            if (Player == null || !Player.gameObject.activeSelf)
            {
                var e = new Assets.Scripts.Manager.Event();
                e.Time = GlobalFunctions.GetTime();
                e.Action = string.Format("got {1} {0}", policy.RewardType, policy.Reward);
                e.Ant = target.AntType.ToString();
                e.Description = string.Format("{0} {1} {2}", policy.Option, policy.ComparisonOperator, policy.Value);
                EnvironmentManager.Instance.NewsManager.Events.Add(e);

                for (float i = policy.Reward; i > 0; i--)
                {
                    //if  (Random.value > 0.5f) return; // TODO: Implement an intelligent norm execution handler
                    if (policy.RewardType == RewardType.Incentive)
                    {
                        target.IncreaseRank();
                    }
                    else
                    {
                        target.DecreaseRank();
                    } 
                }
                target.UpdateValues();
                return;
            }

            // # Player-based Norm Execution
            if (TaskPanel.ActiveTasks.Count >= FlowManager.Flow.MaximumEnemies) return;
            var task = (GameObject) Instantiate(TaskPrefab);
            var t = task.GetComponent<Task>();
            t.transform.SetParent(TaskPanel.transform, false);
            t.Target = target;
            t.Policy = policy;
            TaskPanel.ActiveTasks.Add(t);
        }
    

    

        public void FlashDamage()
        {
            Flash = true;
            MyCg.alpha = 0.5f;
        }

        public void FadeDisplay()
        {
            if (Flash)
            {
                MyCg.alpha = MyCg.alpha - Time.deltaTime;
                if (MyCg.alpha <= 0)
                {
                    MyCg.alpha = 0;
                    Flash = false;
                }
            }
        }
    }
}
