using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Tutorial
{
    public class TutorialIntro : MonoBehaviour
    {
        public Text Hello;
        public Text Intro;
        public Text Description;

        public AudioSource Hey;
        public AudioSource Explanation;

        public float FadeTime = 3;
        public int ShowIntroAfter = 2;
        public int ShowDescriptionAfter = 5;

        // Use this for initialization
        void Start()
        {
            // Hide all
            Hello.CrossFadeAlpha(0.0f,0,false);
            Intro.CrossFadeAlpha(0.0f, 0, false);
            Description.CrossFadeAlpha(0.0f, 0, false);

            // Show Intro
            Hello.CrossFadeAlpha(1.0f, 1, false);

            // Show Others
            StartCoroutine_Auto(ShowIntro());
            StartCoroutine_Auto(ShowDescription());

            // Start Audio
            PlaySound();
        }

        private void PlaySound()
        {
            Hey.Play();
            Explanation.PlayDelayed(Hey.clip.length);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private IEnumerator ShowIntro()
        {
            yield return new WaitForSeconds(ShowIntroAfter);
            Intro.CrossFadeAlpha(1.0f, FadeTime, false);
        }

        private IEnumerator ShowDescription()
        {
            yield return new WaitForSeconds(ShowDescriptionAfter);
            Description.CrossFadeAlpha(1.0f, FadeTime, false);
        }


    }
}
