using Assets.Scripts.Manager;
using UnityEngine;
using GameManager = FlipWebApps.GameFramework.Scripts.GameStructure.GameManager;

namespace Assets.Scripts.Player
{
    public enum FoodType
    {
        Hamburger,
        Cake,
        Donut,
        IceCream,
        Muffin,
        Waffle
    }

    public class Food : MonoBehaviour
    {
        public FoodType Type;
        public Material Mesh;
        public bool IsJob = true;
        public bool Activated;


        // Use this for initialization
        void Start()
        {
            Mesh = GetComponent<MeshRenderer>().material;
        }

        // Update is called once per frame
        void Update()
        {
            if (IsJob && !Activated)
            {
                float ttl = Random.Range(30, 120);
                if (EnvironmentManager.Instance.FlowManager)
                    ttl = ttl * (1 + EnvironmentManager.Instance.FlowManager.FlowLevel);
                Invoke("SelfDestruction", ttl);
                Activated = true;
            }
        }

        void SelfDestruction()
        {
            Destroy(gameObject);
        }

        public void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.tag == "Player" && GetComponent<MeshRenderer>().material.name == "Red (Instance)")
            {
                col.gameObject.GetComponent<AntEater>().IncreaseScore();
                transform.SetParent(col.gameObject.transform.FindChild("Inventory"));
                GetComponent<MeshRenderer>().material = Mesh;
                GetComponent<Rigidbody>().isKinematic = false;
                GetComponent<BoxCollider>().isTrigger = false;
                gameObject.SetActive(false);
                CancelInvoke("SelfDestruction");
                IsJob = false;
            }
        }
    }
}
