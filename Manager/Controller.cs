using System;
using System.Linq;
using Assets.Scripts.Jobs;
using Assets.Scripts.SettingClasses;
using UnityEngine;
using Assets.Scripts.Ant.AgentTypes;
using Assets.Scripts.UI.Development;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Manager
{
    public class Controller : MonoBehaviour
    {
        public GameObject AntContainer;
        public GameObject JobPrefab;
        public GameObject AntPrefab;
        public GameObject JobContainer;
        public Texture2D CursorTexture;
        public GameObject Statistics;

        public bool Autorun;
        public bool Restart;
        public bool NormsEnabled;
        public float Duration = 1800;
        public float NormsAfter = 750;
        public string Setting = "setting.json";

        private Setting _settings = EnvironmentManager.Instance.Settings;

        public void Start()
        {
            EnvironmentManager.Instance.Reset();
            Application.runInBackground = true;

            // Define Simulation

            EnvironmentManager.Instance.CurrentSetting = Setting;
            _settings = EnvironmentManager.Instance.Settings;

            // Spawn Ants

            // Spawn Ants for each Ant Type Setting
            foreach (var antTypeSetting in _settings.AntTypeSettings)
            {
                SpawnAntForAntType(antTypeSetting);
            }
            InvokeRepeating("SpawnJob", 1, 5);
            Invoke("InitAnts", 3);
            //Vector2 CursorCenter = new Vector2(CursorTexture.height / 2, CursorTexture.width / 2);
            //Cursor.SetCursor(CursorTexture, CursorCenter, CursorMode.Auto);

            // Statistical View
            if (Autorun)
            {
                Statistics.gameObject.SetActive(true);
                Time.timeScale = 15;
                Invoke("Reset", Duration);
            }
            if (NormsEnabled)
            {
                if (Statistics)
                    Statistics.GetComponent<SpeedupPerTimeSpan>().Category = "Norms";
                ModeNorms();
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                var cam = GameObject.Find("MainCamera").GetComponent<SmoothFollow>();
                cam.target = null;
            }
        }

        public void ModeNorms()
        {
            Invoke("EnableNorm", NormsAfter);
        }

        public void EnableNorm()
        {
            EnvironmentManager.Instance.NormManager.Norms.Find(n => n.Name == "Detect Cunning Agents").IsActive = true;
            //EnvironmentManager.Instance.NormManager.Norms.Find(n => n.Name == "Reject Cunnings").IsActive = true;
        }

        public void Reset()
        {
            Statistics.GetComponent<SpeedupPerTimeSpan>().SaveData();
            Statistics.gameObject.SetActive(false);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_Development", LoadSceneMode.Single);
        }

        public void InitAnts()
        {
            foreach (var ant in GameObject.Find("Hive").transform.GetComponentsInChildren<AntAI>())
            {
                ant.Agent.Ant = ant;
            }
        }

        public void SpawnJob()
        {
            var ants = AntContainer.transform.GetComponentsInChildren<AntAI>();
            if (!ants.Any()) return;

            AntAI ant = ants.ElementAt(Random.Range(0, ants.Length - 1));
            int enqueuedJobs = ant.Agent.Jobs.EnqueuedJobs.transform.childCount;

            // Check if Space for Job is available
            if (enqueuedJobs >= EnvironmentManager.Instance.Settings.Capacity) return;

            Transform transPendingSlot = ant.Agent.Jobs.EnqueuedJobs.transform;
            GameObject goJob = CreateNewJob(enqueuedJobs);
            Job newJob = goJob.GetComponent<Job>();
            newJob.Owner = ant.Agent;
            newJob.GenerateRandomData();
            goJob.transform.SetParent(transPendingSlot, false);
        }

        public GameObject CreateNewJob(int childCount)
        {
            return (GameObject)Instantiate(JobPrefab, new Vector3(0.0f, (float)childCount * JobPrefab.transform.localScale.y, 0.0f), Quaternion.identity);
        }
        

        public void ToggleJobSpawning(bool value)
        {
            if (value)
                InvokeRepeating("SpawnJob", 0, _settings.JobSetting.JobSpawnRate);
            else
                CancelInvoke("SpawnJob");
        }

        public void SpawnAntForAntType(AntTypeSetting setting)
        {
            foreach (Groups group in Enum.GetValues(typeof(Groups)))
            {
                SpawnAnts(group, setting);
            }
        }

        public void SpawnAnts(Groups group, AntTypeSetting setting)
        {
            var factory = new AgentFactory();
            // Get a random spawn point
            Vector3 randomDirection = Random.insideUnitSphere * Terrain.activeTerrain.terrainData.detailWidth/2;
            randomDirection += gameObject.transform.position;
            NavMeshHit hit;
            // Get amount for random spawning
            int randomCount = Random.Range(0, setting.DistributionSetting.RandomCount.Find(n => n.AntGroup == group).Count);

            // Spawn ants
            for (int i = 0; i < setting.DistributionSetting.FixedCount.Find(n => n.AntGroup == group).Count + randomCount; i++)
            {
                // Find an appropriate spawning place
                for (int j = 0; j < 300; j++)
                {
                    if (!NavMesh.SamplePosition(randomDirection, out hit, _settings.SpawnRadius, 1)) continue;
                    // Get Agent
                    var agent = factory.CreateAgent(group, setting.AntType);
                    // Instantiate Object
                    var newAnt = (GameObject)Instantiate(AntPrefab, hit.position, Random.rotation);
                    newAnt.GetComponent<AntAI>().Agent = agent;
                    agent.Ant = newAnt.GetComponent<AntAI>();
                    newAnt.transform.SetParent(AntContainer.transform);
                    break;
                }
            }
        }
    }
}
