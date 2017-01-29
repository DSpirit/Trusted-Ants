using Assets.Scripts.UI;
using Assets.Scripts.UI.Development;
using UnityEngine;

namespace Assets.Scripts.Manager
{
    public class UserInterfaceManager : MonoBehaviour
    {
        public GameObject TopPanel;

        public AntPanel AntPanel;
        public JobPanel JobPanel;
        public MeetingUI MeetingUi;
        public NormPanel NormPanel;
        public NewsPanel NewsPanel;
        public StatsPanel StatsPanel;
        public TaskPanel TaskPanel;

        public void Start()
        {
            
        }

        public void Update()
        {
            ListenKeys();
        }

        public void ListenKeys()
        {
            if (Input.GetKeyDown(KeyCode.F10))
            {
                bool isActive = TopPanel.activeSelf;
                TopPanel.SetActive(!isActive);
            }
        }
    }
}
