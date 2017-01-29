using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Ant;
using Assets.Scripts.Ant.AgentTypes;
using Assets.Scripts.Jobs;
using Assets.Scripts.Player;
using Assets.Scripts.SettingClasses;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Manager
{
    [Serializable]
    public class SpawnManager : MonoBehaviour
    {
        public List<GameObject> Prefabs = new List<GameObject>();
        public List<Material> Materials = new List<Material>();
        public List<GameObject> InventoryGameObjects = new List<GameObject>();


        private readonly Setting _settings = EnvironmentManager.Instance.Settings;
        private GameObject _meetings;

        // Use this for initialization
        void Start () {
	        _meetings = GameObject.Find("Meetings");
        }
	
        // Update is called once per frame
        void Update () {
	
        }

        public GameObject SpawnGameObject(string prefab, Vector3 position)
        {
            var p = Prefabs.Find(n => string.Equals(n.name, prefab, StringComparison.CurrentCultureIgnoreCase));
            if (p == null) 
                return null;

            Quaternion rotation = Quaternion.identity;
            GameObject spawnedObject = (GameObject) Instantiate(p, position, rotation);
            return spawnedObject;
        }

        public GameObject SpawnGameObject(GameObject prefab, Vector3 position)
        {
            if (prefab == null)
                return null;

            Quaternion rotation = Quaternion.identity;
            GameObject spawnedObject = (GameObject)Instantiate(prefab, position, rotation);
            return spawnedObject;
        }

        public AntMeeting SpawnMeeting(AntAI submitter, AntAI worker, ActionType meetingType)
        {
            var meetingPoint = Vector3.Lerp(submitter.transform.position, worker.transform.position, 0.5f);
            // Create Game Object with Meeting Points
            var meeting = SpawnGameObject("meeting", GlobalFunctions.PointOnNavmesh(meetingPoint));

            var p1 = meeting.transform.FindChild("Ant_1");
            var p2 = meeting.transform.FindChild("Ant_2");

            float additionalDistance = 0f;
            while (Vector3.Distance(p1.transform.position, p2.transform.position) < 5)
            {
                var offset  = new Vector3(additionalDistance, 0);
                p1.transform.position = Vector3.Lerp(submitter.transform.position, meetingPoint + offset, 0.5f);
                p2.transform.position = Vector3.Lerp(worker.transform.position, meetingPoint - offset, 0.5f);
                additionalDistance++;
            }
            
            meeting.transform.SetParent(_meetings.transform);
            
            var m = meeting.GetComponent<AntMeeting>();
            // Assign Meeting Type (Distribution / Verification)
            m.MeetingType = meetingType;
            // Assign Points
            m.Submitter = submitter.Agent;
            m.Worker = worker.Agent;

            submitter.Meeting = p1;
            worker.Meeting = p2;
            return m;
        }

        public GameObject GetRandomFood()
        {
            GameObject item = InventoryGameObjects.ElementAt(Random.Range(0, InventoryGameObjects.Count - 1));
            return item;
        }

        public List<Food> GetRandomInventory()
        {
            var list = new List<Food>();

            foreach (var food in InventoryGameObjects)
            {
                var amount = Random.Range(1, 11);
                for (int i = 0; i < amount; i++)
                {
                    list.Add(Instantiate(food).GetComponent<Food>());
                }
            }

            return list;
        }
    }
}
