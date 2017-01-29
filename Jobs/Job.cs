using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Ant.AgentTypes;
using Assets.Scripts.Manager;
using Assets.Scripts.Norms;
using Assets.Scripts.SettingClasses;
using UnityEngine;
using GameManager = FlipWebApps.GameFramework.Scripts.GameStructure.GameManager;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Jobs
{ // Four different groups an ant can belong to


    public class Job : MonoBehaviour
    {
        public Guid Id;
        public Guid Parent = Guid.Empty;
        public float Progress;

        [SerializeField]
        public int HopCount;
        [SerializeField]
        public List<JobPart> JobParts;
        public float StartTimestamp;
        public float FinishedTimeStamp;
        [SerializeField]
        public float Ttl;

        public Agent Owner;
        public Contract JobContract;

        // Path Finding
        public float EstimatedEffort; // Estimated effort for one ant
        public NavMeshPath Path;
        private MeshRenderer _bubbleMesh;
        public GameObject JobsContainer;
        public Animator Ani;

        public readonly JobSetting Settings = EnvironmentManager.Instance.Settings.JobSetting;
        private readonly Statistics _statistics = Statistics.Instance(); // Jobtracking

        // Use this for pre-initialization
        void Awake()
        {
            _bubbleMesh = GetComponent<MeshRenderer>();

            Id = Guid.NewGuid();
        }

        public void GenerateRandomData()
        {
            int j = Random.Range(Settings.MinimumJobParts, Settings.MaximumJobParts + 1);

            while (j > 0)
            {
                GameObject partGo = new GameObject("Job Part " + j);
                partGo.transform.parent = transform;
                // generate a Position and marker for every job part
                j--;
                JobPart jobPart = partGo.AddComponent<JobPart>();
                jobPart.SetParent(this);
                JobParts.Add(jobPart);
            }
            StartTimestamp = Time.time;
            _statistics.RegisterJob(this);
            Ttl = Random.Range(Settings.MinimumTimeToLive, Settings.MaximumTimeToLive) * JobParts.Count;
        }

        public void Start()
        {
            Ani = GetComponent<Animator>();
            JobsContainer = GameObject.Find("Done Jobs");
            EstimateEffort();
            JobContract = new Contract(this);
            Owner.Contracts.Add(JobContract);
            InvokeRepeating("UpdateJob", 1, 1);
        }

        // Update is called once per frame
        void Update()
        {
            if (Ani.GetBool("IsActive"))
            {
                Ttl -= Time.deltaTime;
            }
        }

        void UpdateJob()
        {
            CheckParts();
            CheckProgress();
        }

        private void CheckParts()
        {
            JobParts = GetComponentsInChildren<JobPart>().ToList();
        }


        private void EstimateEffort()
        {
            // Calculate Path with Waypoints
            Path = new NavMeshPath();
            Vector3[] waypoints = new Vector3[JobParts.Count];

            // Sort JobParts and set index
            Vector3 lastPos = Owner.Ant.transform.position;

            JobParts.ForEach(n => n.Index = 0);

            for (int i = 0; i < JobParts.Count; i++)
            {
                var pos = lastPos;
                var part = JobParts.Where(l => l.Index == 0).OrderBy(n => Vector3.Distance(pos, n.Position)).First();
                part.Index = i + 1;
                waypoints[i] = part.Position;
                lastPos = part.Position;
            }

            float totalEffort = 0;
            // Calculate Waypoints
            foreach (var waypoint in waypoints)
            {
                Owner.NavAgent.CalculatePath(waypoint, Path);
                for (int i = 1; i < Path.corners.Length; ++i)
                {
                    totalEffort += Mathf.Abs(Vector3.Distance(Path.corners[i - 1], Path.corners[i]));
                }
            }
            // Estimate Effort with partial max speed as default (because of breaking, acceleration etc.)
            EstimatedEffort = totalEffort / (Owner.NavAgent.speed * 0.4f);

        }

        #region State Check Methods
        private void CheckProgress()
        {
            if (JobParts != null)
                Progress = NumberPartsDone() / JobParts.Count;
        }

        public List<JobPart> UnfinishedJobParts()
        {
            return JobParts.Where(n => !n.Finished()).ToList();
        }

        public bool AllPartsDone()
        {
            return JobParts.All(n => n.Finished());
        }

        // return number of JobParts which are done
        public int NumberPartsDone()
        {
            return JobParts.Count(n => n.Finished());
        }

        public bool SuccessfullyFinished()
        {
            return JobParts.All(n => n.Successful);
        }

        public bool Exceeded()
        {
            return Ttl.Equals(-1);
        }
        #endregion

        public Job CloneJob(Agent successor)
        {
            HopCount++;
            // Create a 1:1 copy
            GameObject copyJobGo = EnvironmentManager.Instance.Controller.CreateNewJob(1);
            Job copyJob = copyJobGo.GetComponent<Job>();
            copyJob.Parent = Id;
            copyJob.JobParts.Clear();
            copyJob.StartTimestamp = Time.time;
            copyJob.HopCount = this.HopCount;
            copyJob.Owner = successor;
            copyJob.Ttl = this.Ttl;

            // Split Job
            int count = UnfinishedJobParts().Count / 2;
            var parts = UnfinishedJobParts().Skip(count).ToList();

            foreach (JobPart jobPart in parts)
            {
                jobPart.transform.parent = copyJob.transform;
                JobParts.Remove(jobPart);
                jobPart.SetParent(copyJob);
            }
            copyJob.CheckParts();

            Ani.SetBool("GotDistributed", true);
            JobContract.Initialize(successor, copyJob);

            return copyJob;
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                if (Ani.GetCurrentAnimatorStateInfo(0).IsName("Verified"))
                    GameManager.Instance.Player.AddCoin();
            }
        }
    }
}