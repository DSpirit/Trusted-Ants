using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Player
{
    public class ProgressBar : MonoBehaviour
    {
        public Slider Health;
        public AntAI Ant;
        public void Start()
        {
            Health = GetComponent<Slider>();
            Ant = transform.parent.GetComponentInParent<AntAI>();

        }

        public void Update()
        {
            var cam = Camera.main;
            gameObject.transform.LookAt(cam.transform);
            Health.value = Ant.Agent.Rank;
        }
    }
}
