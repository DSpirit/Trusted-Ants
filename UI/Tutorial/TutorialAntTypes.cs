using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Tutorial
{
    public class TutorialAntTypes : MonoBehaviour
    {
        // Game Objects
        public Text Intro;
        public Text Description;
        public Text LeftColumn;
        public Text RightColumn;

        public AudioSource Look;
        public AudioSource Gorgeous;
        public AudioSource Types;

        // Params
        public float Speed = 0.25F;
        public float ShowDescriptionAfter;
        public float ShowTextsAfter = 8;
        
        public bool Started = false;

        // Internal Params
        private float _startTime;
        private float _journeyLength;

        // Use this for initialization
        void Start()
        {
            // Init Delay
            ShowDescriptionAfter = Look.clip.length;
            ShowTextsAfter = ShowDescriptionAfter + Gorgeous.clip.length;

            // Hide Labels
            Intro.CrossFadeAlpha(0.0f,0,false);
            Description.CrossFadeAlpha(0.0f, 0, false);
            LeftColumn.CrossFadeAlpha(0.0f, 0, false);
            RightColumn.CrossFadeAlpha(0.0f, 0, false);

            // Show Intro
            Intro.CrossFadeAlpha(1.0f, 1, false);

            // Get Timestamp for Lerp
            _startTime = Time.time;
            
            StartCoroutine_Auto(ActivateAnts());
            StartCoroutine_Auto(ShowDescription());
            StartCoroutine_Auto(ShowTexts());

            // Start Sounds
            PlaySound();
        }

        private void PlaySound()
        {
            Look.Play();
            Gorgeous.PlayDelayed(Look.clip.length);
            Types.PlayDelayed(Look.clip.length + Gorgeous.clip.length);
        }

        // Update is called once per frame
        void Update()
        {
            if (Started)
                MoveAnts();
        }

        public IEnumerator ActivateAnts()
        {
            yield return new WaitForSeconds(Look.clip.length);
            Started = true;
        }

        public IEnumerator ShowDescription()
        {
            yield return new WaitForSeconds(ShowDescriptionAfter);
            Description.CrossFadeAlpha(1.0f, 1, false);
        }

        public IEnumerator ShowTexts()
        {
            yield return new WaitForSeconds(ShowTextsAfter);
            LeftColumn.CrossFadeAlpha(1.0f, 1, false);
            RightColumn.CrossFadeAlpha(1.0f, 1, false);
        }

        public void MoveAnts()
        {
            var ants = GameObject.FindGameObjectsWithTag("Ant");
            List<Transform> places = GameObject.Find("Placement").transform.Cast<Transform>().ToList();
            foreach (GameObject ant in ants)
            {
                var target = places.Find(n => n.name == ant.name).transform.position;
                _journeyLength = Vector3.Distance(ant.transform.position, target);
                float distCovered = (Time.time - _startTime) * Speed;
                float fracJourney = distCovered / _journeyLength;
                ant.transform.position = Vector3.Lerp(ant.transform.position, target, fracJourney);
            }
        }
    }
}
