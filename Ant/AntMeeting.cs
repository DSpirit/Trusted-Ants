using System;
using System.Globalization;
using Assets.Scripts.Ant.AgentTypes;
using Assets.Scripts.Manager;
using UnityEngine;

namespace Assets.Scripts.Ant
{
    public enum MeetingState
    {
        Setup, Talking, Finished
    }

    [Serializable]
    public class AntMeeting : MonoBehaviour
    {
        public Agent Submitter;
        public Agent Worker;
        public ActionType MeetingType;
        public Transform Point1;
        public Transform Point2;

        public void Awake()
        {
            Point1 = transform.FindChild("Ant_1");
            Point2 = transform.FindChild("Ant_2");
        }

        public void Update()
        {
            float minDistance = 1.5f;
            bool iArrived = false, partnerArrived = false;
            try
            {
                iArrived = Vector3.Distance(Submitter.Ant.transform.position, Point1.transform.position) < minDistance;
                partnerArrived = Vector3.Distance(Worker.Ant.transform.position, Point2.transform.position) < minDistance;
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }

            bool bothArrived = iArrived && partnerArrived;
            if (bothArrived)
            {
                Submitter.LookAt(Point2);
                Worker.LookAt(Point1);
                Submitter.Ant.Ani.SetTrigger("Arrived");
                Worker.Ant.Ani.SetTrigger("Arrived");
            }
        }

        public void Finished()
        {
            Destroy(gameObject);
        }
    }
}
