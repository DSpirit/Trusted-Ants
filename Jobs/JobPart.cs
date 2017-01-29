using System;
using System.Collections;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Manager;
using UnityEngine;

namespace Assets.Scripts.Jobs
{
    [Serializable]
    public class JobPart : MonoBehaviour
    {
        public Guid Id;
        public int Index;
        // parent Job
        Job _parent;
        // Showing the places the job is located at
        public GameObject MarkerPrefab;
        //// The markers are not part of an ant, but are related to exactly one job
        public GameObject Marker; 

        // Color of the marker
        public Material SuccessColor;
        public Material FailColor;
        public Vector3 Position;
        private MeshRenderer _markerMesh;

        public float StartTimestamp;
        public float FinishTimeStamp = 0.0f;

        public bool Successful;

        private readonly Statistics _statistics = Statistics.Instance();
        private GameObject _jobsContainer;
        private SpawnManager _spawn;


        // initiate Job
        public void Awake()
        {
            _spawn = EnvironmentManager.Instance.SpawnManager;

            MarkerPrefab = _spawn.Prefabs.Find(n => n.name == "MarkerPrefab");
            SuccessColor = _spawn.Materials.Find(n => n.name == "Green");
            FailColor = _spawn.Materials.Find(n => n.name == "Red");

            
            Id = Guid.NewGuid();
            Position = GlobalFunctions.RandomPointOnNavmesh(Terrain.activeTerrain.terrainData.detailWidth/2);
            _jobsContainer = GameObject.Find("Work Units");
            Init();
        }

        public void Start()
        {
           
                
        }

        public void Init()
        {
            Marker = _spawn.SpawnGameObject(_spawn.GetRandomFood(), Position);
            Marker.transform.SetParent(_jobsContainer.transform);
            Marker.name = Id.ToString();
            _markerMesh = Marker.GetComponent<MeshRenderer>();
        }
        
        public void SetParent(Job parent)
        {
            _parent = parent;
            
        }

        // Update is called once per frame
        void Update ()
        {
            if (!Marker)
                FinishJobPart();
        }
      
        public void ProcessJobPart()
        {
            if (StartTimestamp < 0.01f)
            {
                _parent.Ani.SetBool("IsActive", true);
                StartTimestamp = Time.time;
                _statistics.JobStatisticsList.Find(n => n.Id == _parent.Id).RegisterJobPart(this);
            }
        }

        public void FinishJobPart()
        {
           Successful = _parent.Ttl > 0;
            if (Marker)
                _markerMesh.material = Successful ? SuccessColor : FailColor;
            if (FinishTimeStamp < 1)
            {
                FinishTimeStamp = Time.time;
                _statistics.FinishedJobPart(this, Successful);
            }
            
            //CleanupJobPart();
            if (!Successful)
            {
                Marker = null;
            }
            if (Marker)
                Destroy(Marker, 1);
        }

        public bool Finished()
        {
            return !Marker;
        }

        public float Duration()
        {
            return FinishTimeStamp - StartTimestamp;
        }
    }
}

