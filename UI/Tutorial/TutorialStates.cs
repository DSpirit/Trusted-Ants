using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Tutorial
{
    public class TutorialStates : MonoBehaviour
    {
        public Text Intro;
        public List<RawImage> Images;
        public List<Text> Descriptions;

        public AudioSource Explanation;
        public AudioSource States;

        // Use this for initialization
        void Start()
        {
            Images = GetComponentsInChildren<RawImage>().ToList();
            Descriptions = GetComponentsInChildren<Text>().ToList();

            

            int after = (int) Explanation.clip.length;

            foreach (var image in Images)
            {
                image.CrossFadeAlpha(0.0f, 0, false);
                StartCoroutine_Auto(ShowImages(image, after));
                after = after+2;
            }
            after = (int)Explanation.clip.length;
            foreach (var text in Descriptions)
            {
                text.CrossFadeAlpha(0.0f, 0, false);
                StartCoroutine_Auto(ShowTexts(text, after));
                after = after+2;
            }
            Intro.CrossFadeAlpha(1.0f, 1, false);

            PlaySound();
        }

        private void PlaySound()
        {
            Explanation.Play();
            States.PlayDelayed(Explanation.clip.length);
        }

        public IEnumerator ShowTexts(Text text, int after)
        {
            yield return new WaitForSeconds(after);
            text.CrossFadeAlpha(1.0f, 2, false);
        }

        public IEnumerator ShowImages(RawImage image, int after)
        {
            yield return new WaitForSeconds(after);
            image.CrossFadeAlpha(1.0f, 2, false);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
