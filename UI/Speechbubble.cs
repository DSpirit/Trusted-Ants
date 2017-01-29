using Assets.Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class Speechbubble : MonoBehaviour
    {
        public RawImage BubbleImageComponent;
        public Texture StateWaiting;
        public Texture StateExploring;
        public Texture StateWorking;
        public Texture StateGoToMeeting;
        public Texture StateTalking;
        public Texture StateDistributing;
        public Texture StateVerifying;
        public Texture StateExitMeeting;
        public Texture StateDeath;
        public Texture StateEating;
        public Texture StateFighting;
        public Texture2D Healthbar;


        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            var cam = Camera.main;
            gameObject.transform.rotation = cam.transform.rotation;
        }

        public void Exploring() //method to set our first image
        {
            BubbleImageComponent.texture = StateExploring;
        }

        public void Waiting() //method to set our first image
        {
            BubbleImageComponent.texture = StateWaiting;
        }

        public void Working() //method to set our first image
        {
            BubbleImageComponent.texture = StateWorking;
        }

        public void Talking() //method to set our first image
        {
            BubbleImageComponent.texture = StateTalking;
        }

        public void Verifying() //method to set our first image
        {
            BubbleImageComponent.texture = StateVerifying;
        }

        public void Distributing() //method to set our first image
        {
            BubbleImageComponent.texture = StateDistributing;
        }

        public void ExitMeeting() //method to set our first image
        {
            BubbleImageComponent.texture = StateExitMeeting;
        }

        public void GoToMeeting()
        {
            BubbleImageComponent.texture = StateGoToMeeting;
        }

        public void Death()
        {
            BubbleImageComponent.texture = StateDeath;
        }

        public void Eating()
        {
            BubbleImageComponent.texture = StateEating;
        }

        public void Fighting()
        {
            BubbleImageComponent.texture = StateFighting;
        }

        public void UpdateHealth(float health)
        {
            //BubbleImageComponent.texture = new Texture2D((int) HealthBarSize.x, (int) HealthBarSize.y);
        }
        
        public Vector2 HealthBarSize = new Vector2(100,20);
        
    }
}