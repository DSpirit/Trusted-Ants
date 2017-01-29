using Assets.Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Development
{
    public class MeetingUI : MonoBehaviour, IPanel
    {
        public Text Meetings;

        private EnvironmentManager _manager;
        public void Start()
        {
            _manager = EnvironmentManager.Instance;
            InvokeRepeating("ClearDisplay", 20, 15);
        }

        public void Update()
        {
            
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void ClearDisplay()
        {
            Meetings.text = "";
        }
    }
}
