using Assets.Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Player
{
    public class Healthbar : MonoBehaviour
    {

        public int maxHealth = 100;
        public int curHealth = 100;
        public float healthBarLength;
        public GUIStyle Full;
        public GUIStyle Empty;
        

        // Use this for initialization
        void Start()
        {
            healthBarLength = Screen.width / 6;
        }

        // Update is called once per frame
        void Update()
        {
            AddjustCurrentHealth(0);
        }

        void OnGUI()
        {
            try
            {
                Vector3 screenPosition = Camera.current.WorldToScreenPoint(transform.position);
                screenPosition.y = Screen.height - (screenPosition.y + 1); // inverts y
                Rect empty = new Rect(screenPosition.x - 50, screenPosition.y - 12, 100, 10);
                // makes a rect centered at the player ( 100x24 )
                Rect full = new Rect(screenPosition.x - 50, screenPosition.y - 12, curHealth, 10);
                // makes a rect centered at the player ( 100x24 )
                GUI.Box(empty, "", Empty);
                GUI.Box(full, "", Full);
            }
            catch
            {
                
            }
            
        }

        public void AddjustCurrentHealth(int adj)
        {
            curHealth += adj;

            if (curHealth < 0)
                curHealth = 0;

            if (curHealth > maxHealth)
                curHealth = maxHealth;

            if (maxHealth < 1)
                maxHealth = 1;

            healthBarLength = (Screen.width / 6) * (curHealth / (float)maxHealth);
        }
    }
}