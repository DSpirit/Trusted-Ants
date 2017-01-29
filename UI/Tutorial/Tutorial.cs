using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Assets.Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Assets.Scripts.UI.Tutorial
{
    public enum TutorialSteps
    {
        Intro, AntTypes, States, Jobs, Navigation
    }

    public class Tutorial : MonoBehaviour, IPanel
    {
        public GameObject Intro;
        public GameObject AntTypes;
        public GameObject States;
        public GameObject Jobs;
        public GameObject Navigation;
        public Button NextButton;

        private TutorialSteps _currentStep = TutorialSteps.Intro;
        private bool _nextStep;
        private GameObject _sounds;
        

        public void Start()
        {
            _sounds = GameObject.Find("Sounds");
            _nextStep = true;
        }

        public void Update()
        {
            if (!_nextStep) return;
            try
            {
                _nextStep = false;
                foreach (TutorialSteps step in Enum.GetValues(typeof(TutorialSteps)))
                {
                    bool active = step == _currentStep;
                    var child = transform.FindChild(step.ToString());
                    child.gameObject.SetActive(active);

                    // Enable / Disable Sounds
                    
                    foreach (var sound in _sounds.transform.GetComponentsInChildren<AudioSource>(true))
                    {
                        int l = (int) step + 1;
                        if (sound.name.Contains(l + "_"))
                            sound.gameObject.SetActive(active);
                    }

                    if (active && step == TutorialSteps.Navigation)
                        NextButton.enabled = false;
                }
            }
            catch
            {
                
            }
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void ReturnToMenu()
        {
            SceneManager.LoadScene("Menu");   
        }

        public void NextStep()
        {
            int currentStep = (int)_currentStep;
            _currentStep = (TutorialSteps)currentStep + 1;
            _nextStep = true;
        }
    }
}
