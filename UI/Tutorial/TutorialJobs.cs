using System.Collections;
using Assets.Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Tutorial
{
    public class TutorialJobs : MonoBehaviour
    {
        public Text Intro;
        public Text Description;

        public AudioSource Explanation;
        public AudioSource Jobs;
        public AudioSource Parts;
        public AudioSource Outro;

        public float Wait;
        public int FadeTime = 1;

        private bool _showDetails;
        // Use this for initialization
        void Start()
        {
            Wait = Explanation.clip.length + Jobs.clip.length + Parts.clip.length;

            Intro.CrossFadeAlpha(0.0f,0,false);
            Description.CrossFadeAlpha(0.0f,0,false);
            Intro.CrossFadeAlpha(1.0f, FadeTime, false);
            StartCoroutine_Auto(ShowDescription());

            PlaySound();
        }

        private void PlaySound()
        {
            Explanation.Play();
            Jobs.PlayDelayed(Explanation.clip.length);
            Parts.PlayDelayed(Explanation.clip.length + Jobs.clip.length);
            Outro.PlayDelayed(Wait);
        }

        // Update is called once per frame
        void Update()
        {
            if (_showDetails)
                MoveText();
        }

        IEnumerator ShowDescription()
        {
            yield return new WaitForSeconds(Wait);
            _showDetails = true;
        }

        private void MoveText()
        {
            Intro.CrossFadeAlpha(0.0f, FadeTime, false);
            Description.CrossFadeAlpha(1.0f, FadeTime, false);
            Description.transform.position = Vector3.Lerp(Description.transform.position, Intro.transform.position, Time.deltaTime);
        }
    }
}
