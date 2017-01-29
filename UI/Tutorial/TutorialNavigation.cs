using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Tutorial
{
    public class TutorialNavigation : MonoBehaviour
    {
        public Text Intro;
        public Text Description;

        public AudioSource Expert;
        public AudioSource Play;

        public int ShowDetailsAfter = 5;
        public int ShowDescriptionAfter = 15;
        public List<Text> Texts;
        
        // Use this for initialization
        void Start()
        {
            Description.CrossFadeAlpha(0.0f, 0, false);
            Texts = transform.GetComponentsInChildren<Text>().Where(n => n.name.Contains("Text")).ToList();
            foreach (var text in Texts)
            {
                text.CrossFadeAlpha(0.0f, 0, false);
            }
            Intro.CrossFadeAlpha(1.0f,1,false);
            StartCoroutine_Auto(ShowDetails());
            StartCoroutine_Auto(ShowDescription());

            PlaySound();
        }

        private void PlaySound()
        {
            Expert.Play();
            Play.PlayDelayed(Expert.clip.length);
        }

        public IEnumerator ShowDetails()
        {
            yield return new WaitForSeconds(ShowDetailsAfter);
            int fadeInAfter = 1;
            foreach (var text in Texts)
            {
                text.CrossFadeAlpha(1.0f, fadeInAfter, false);
                fadeInAfter++;
            }
        }

        public IEnumerator ShowDescription()
        {
            yield return new WaitForSeconds(ShowDescriptionAfter);
            Description.CrossFadeAlpha(1.0f, 1, false);

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
