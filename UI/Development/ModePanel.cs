using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Manager;
using Assets.Scripts.Player;
using Assets.Scripts.UI.Player;
using UnityEngine;

namespace Assets.Scripts.UI.Development
{
    public class ModePanel : MonoBehaviour, IPanel
    {
        public ItemPanel Items;
        public AntEater AntEater;
        public Beetle Beetle;
        public PlayerPanel PlayerPanel;


        public void Start()
        {
            CheckApplicationMode();
        }

        public void Update()
        {
            ListenKeys();
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void Retry()
        {
            FindObjectOfType<SceneManager>().ChangeToScene("Menu");
        }

        public void Cancel()
        {
            Application.Quit();
        }

        public void PlayAnteater()
        {
            PlayerPanel.gameObject.SetActive(true);
            Items.gameObject.SetActive(true);
            AntEater.gameObject.SetActive(true);
            //Beetle.gameObject.SetActive(false);
            Camera.main.GetComponent<SmoothFollow>().target = AntEater.transform.FindChild("Head");
            var panel = GameObject.Find("TopPanel");
            panel.SetActive(false);
            gameObject.SetActive(false);
            AntEater.Scores.gameObject.SetActive(false);

            foreach (var ant in EnvironmentManager.Instance.Ants)
            {
                ant.StateBubble.gameObject.SetActive(false);
            }
        }

        public void PlayBeetle()
        {
            Beetle.gameObject.SetActive(true);
            AntEater.gameObject.SetActive(false);
            Camera.main.GetComponent<SmoothFollow>().target = Beetle.transform.FindChild("Head");
        }

        public void ResetGame()
        {
            try
            {
                Camera.main.GetComponent<SmoothFollow>().target = null;
                AntEater.gameObject.SetActive(false);
                Items.gameObject.SetActive(false);
                PlayerPanel.gameObject.SetActive(false);
            }
            catch (Exception)
            {
               
            }
            
        }

        public void ListenKeys()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ResetGame();
            }
        }

        public void CheckApplicationMode()
        {
            ApplicationMode mode = (ApplicationMode)PlayerPrefs.GetInt("Mode", 1);
            if (mode == ApplicationMode.Development)
            {
                ResetGame();
            }
            else
            {
                PlayAnteater();
            }
        }


    }
}
