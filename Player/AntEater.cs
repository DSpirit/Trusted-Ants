using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Manager;
using Assets.Scripts.SettingClasses;
using Assets.Scripts.UI.Player;
using FlipWebApps.GameFramework.Scripts.UI.Dialogs.Components;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

namespace Assets.Scripts.Player
{
    public enum ViewMode
    {
        FirstPerson, ThirdPerson
    }

    public class AntEater : MonoBehaviour
    {
        public Camera MainCamera;
        public FlowSetting Flow;
        public FlowManager FlowManager;
        public GameManager Game;
        public ViewMode CameraView;
        public List<Food> Inventory;
        public ParticleSystem Soaker;
        public ItemPanel Items;
        public Transform Bag;
        public Transform Tasks;
        public Animator Animation;
        public float Health = 1.0f;
        public float Mana = 1.0f;
        public float SpeedBooster = 0.5f;
        public float Cooldown;
        public const float CooldownTime = 3.0f;
        public float Force = 1f;
        public ForceMode ForceMode;
        public PlayerStats Stats;
        public float SpeedDecrease = 0.2f;
        public ScorePanel Scores;

        private ThirdPersonCharacter _motor;
        private float _defaultEmission;
        private float _defaultParticleScale;

        
        void Start()
        {
            FlowManager = EnvironmentManager.Instance.FlowManager;
            Stats = new PlayerStats();
            Flow = FlowManager.Flow;
            
            Game = EnvironmentManager.Instance.GameManager;

            Bag = transform.FindChild("Inventory");
            Animation = GetComponent<Animator>();

            // Get Defaults
            _motor = GetComponent<ThirdPersonCharacter>();
            _defaultEmission = Soaker.emission.rate.constant;
            _defaultParticleScale = Soaker.collision.radiusScale;

            GetInventory();
            SetViewMode(ViewMode.ThirdPerson);
            var lvl = (CustomLevel) FlipWebApps.GameFramework.Scripts.GameStructure.GameManager.Instance.Levels.Selected;
            int setting = lvl.Setting;
            var instance = DialogManager.Instance.ShowInfo(textKey: "Level." + setting, text2Key: "Level.Description." + setting);
            instance.GetComponentInChildren<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        }

        private void GetInventory()
        {
            Inventory = EnvironmentManager.Instance.SpawnManager.GetRandomInventory();
            foreach (var o in Inventory)
            {
                o.transform.position = Bag.transform.position;
                o.transform.SetParent(Bag);
                o.gameObject.SetActive(false);
            }
            Items = FindObjectOfType<ItemPanel>();
        }

        // Update is called once per frame
        void Update()
        {
            Listen();
            CheckItems();
            Health = FlipWebApps.GameFramework.Scripts.GameStructure.GameManager.Instance.Player.Health;

            if (Cooldown > 0)
                Cooldown -= Time.deltaTime;

            if (Health <= 0)
            {
                
            }
        }

        public void AdjustFlowValues()
        {
            // Adjust Size
            Soaker.startSize = 15 * (2 - FlowManager.FlowLevel);

            // Adjust Bursts
            int bursts = (int)(4 - FlowManager.FlowLevel);
            ParticleSystem.Burst[] b = new ParticleSystem.Burst[bursts];
            for (int i = 0; i < bursts; i++)
            {
                b[i] = new ParticleSystem.Burst((float)1 / (i + 1), (short)((i + 1) * 15));
            }
            Soaker.emission.SetBursts(b);
            // Adjust Force
            Force = Force*(1 + FlowManager.FlowLevel);
        }

        private void CheckItems()
        {
            Inventory = Bag.GetComponentsInChildren<Food>(true).ToList();
            Inventory.ForEach(n => n.GetComponent<BoxCollider>().isTrigger = false);
            Inventory.ForEach(n => n.GetComponent<Rigidbody>().isKinematic = false);


            Items.BurgerAmount.text = Inventory.Count(n => n.Type == FoodType.Hamburger).ToString();
            Items.CakeAmount.text = Inventory.Count(n => n.Type == FoodType.Cake).ToString();
            Items.DonutAmount.text = Inventory.Count(n => n.Type == FoodType.Donut).ToString();
            Items.MuffinAmount.text = Inventory.Count(n => n.Type == FoodType.Muffin).ToString();
            Items.IceAmount.text = Inventory.Count(n => n.Type == FoodType.IceCream).ToString();
            Items.WaffleAmount.text = Inventory.Count(n => n.Type == FoodType.Waffle).ToString();

            if (Bag.childCount < 1) return;
            if (Inventory.All(n => n.Type != Items.CurrentFoodType))
                Items.SetFoodType((int)Inventory.First().Type);
        }

        public void GotHit(float value)
        {
            FlipWebApps.GameFramework.Scripts.GameStructure.GameManager.Instance.Player.Health -= value;
            EnvironmentManager.Instance.GameManager.FlashDamage();
        }

        public void IncreaseScore(int value = 1)
        {
            FlipWebApps.GameFramework.Scripts.GameStructure.GameManager.Instance.Levels.Selected.Score += (int)(value * FlowManager.Flow.FlowMultiplier);
        }

        void Listen()
        {
            // Firing

            if (Input.GetButtonDown("Fire2"))
            {
                Throw();
            }
            if (Input.GetButtonDown("Fire1"))
            {
                Soak();
            }

            // Increase / Decrease Speed
            if (Math.Abs(Input.GetAxis("Z Axis")) > 0.05)
            {
                if (SpeedBooster > 0)
                {
                    _motor.SetMultiplier(1 + Math.Abs(Input.GetAxis("Z Axis")));
                    SpeedBooster -= SpeedDecrease*Time.deltaTime;
                }
            }
            else
            {
                _motor.SetMultiplier(1);
            }

            // Item Selection

            int k = 257; // Numeric Block
            int a = 49; // Alphanumeric Block
            for (int i = 0; i < 6; i++)
            {
                if (Input.GetKeyDown((KeyCode)k) || Input.GetKeyDown((KeyCode)a))
                    Items.SetFoodType(i);
                k++;
                a++;
            }

            // Gamespeed
            if (Input.GetKey(KeyCode.LeftControl) && (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus)))
            {
                Time.timeScale += 1;
            }
            if (Input.GetKey(KeyCode.LeftControl) && (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus)))
            {
                Time.timeScale -= 1;
            }
            if (Input.GetButtonDown("Camera"))
            {
                var mode = CameraView == ViewMode.FirstPerson ? ViewMode.ThirdPerson : ViewMode.FirstPerson;
                CameraView = mode;
                SetViewMode(mode);
            }
            if (Input.GetButton("Aim"))
            {
                AimTarget();                
            }
        }

        public void AimTarget()
        {
            if (Game.TaskPanel.ActiveTasks.Any())
            {
                var nearest = Game.TaskPanel.ActiveTasks.OrderBy(n => Vector3.Distance(n.Target.Ant.transform.position, transform.position)).First().Target.Ant.transform.position;
                if (Vector3.Distance(nearest, transform.position) > 20)
                    transform.LookAt(nearest);
            }
            
        }

        void SetViewMode(ViewMode mode)
        {
            if (mode == ViewMode.FirstPerson)
            {
                Camera.main.GetComponent<SmoothFollow>().distance = 20;
                Camera.main.GetComponent<SmoothFollow>().height = 5;
            }
            if (mode == ViewMode.ThirdPerson)
            {
                Camera.main.GetComponent<SmoothFollow>().distance = 50;
                Camera.main.GetComponent<SmoothFollow>().height = 20;
            }
        }

        public void Throw(int force = 10)
        {
            if (Bag.childCount < 1) return;
            if (Animation.GetCurrentAnimatorStateInfo(0).IsName("Punch")) return;
            Animation.SetTrigger("Punch");
            Force = force;
            ThrowFood();
        }

        public void Soak()
        {
            // Cooldown
            if (Cooldown > 0 || Mana <= 0) return;
            Soaker.Play();
            FlowManager.SoakingParticlesEmitted += Soaker.emission.rate.constant;
            Mana -= Flow.ManaConsumption * FlowManager.FlowLevel;
        }

        void ThrowFood()
        {
            var food = Inventory.First(n => n.Type == Items.CurrentFoodType);

            var ray = MainCamera.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2));
            var vhit = new RaycastHit();
            if (Physics.Raycast(ray, out vhit, 1000))
            {
                food.transform.position = transform.FindChild("Head").transform.position;
                food.transform.SetParent(null);
                food.gameObject.SetActive(true);
                food.transform.LookAt(vhit.point);
                food.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * Force, ForceMode);
                FlowManager.TotalThrows++;
            }
            GlobalFunctions.DrawLine(gameObject.transform.position, vhit.point, Color.green);
        }

        public void OnParticleCollision(GameObject other)
        {
            if (other.tag == "Fire")
            {
                GotHit(0.01f*Flow.FlowMultiplier);
            }
        }

        public void IncreaseHealth(float value)
        {
            FlipWebApps.GameFramework.Scripts.GameStructure.GameManager.Instance.Player.Health = Health + value > 1 ? 1 : Health + value;
        }

        public void IncreaseMana(float value)
        {
            Mana = Mana + value > 1 ? 1 : Mana + value;
        }

        public void IncreaseSpeedBooster(float value)
        {
            SpeedBooster = SpeedBooster + value > 1 ? 1 : SpeedBooster + value;
        }
    }
}
