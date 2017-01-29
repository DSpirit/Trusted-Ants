using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Manager
{
    public class SceneManager : MonoBehaviour
    {

        void Update()
        {
            //if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Jump"))
             //   GetUser();
        }

        public void PlayGame()
        {
           
        }

        public void GetUser()
        {
            PlayerPrefs.SetInt("Mode", (int)ApplicationMode.Game);
            
            ChangeToScene("Scene_Development");
        }

        public void PlayDevelopment()
        {
            PlayerPrefs.SetInt("Mode", (int)ApplicationMode.Development);
            ChangeToScene("Scene_Development");
        }

        public void ChangeToScene (string sceneChangeTo) {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneChangeTo);
        }

        public void Quit() {
            Application.Quit();
        }
    }

    public enum ApplicationMode
    {
        Game, Development
    }
}
