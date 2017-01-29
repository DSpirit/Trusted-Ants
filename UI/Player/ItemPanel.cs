using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Player
{
    public class ItemPanel : MonoBehaviour
    {
        public FoodType CurrentFoodType;
        public GameObject Current;
        public GameObject Cake;
        public GameObject Burger;
        public GameObject Ice;
        public GameObject Muffin;
        public GameObject Waffle;
        public GameObject Donut;

        public List<Text> Amounts;
        public Text CakeAmount;
        public Text BurgerAmount;
        public Text IceAmount;
        public Text MuffinAmount;
        public Text WaffleAmount;
        public Text DonutAmount;

        


        void Start()
        {
            //CakeAmount = Cake.GetComponentInChildren<Text>();
            //BurgerAmount = Burger.GetComponentInChildren<Text>();
            //IceAmount = Ice.GetComponentInChildren<Text>();
            //MuffinAmount = Muffin.GetComponentInChildren<Text>();
            //WaffleAmount = Waffle.GetComponentInChildren<Text>();
            //DonutAmount = Donut.GetComponentInChildren<Text>();

            Amounts = GetComponentsInChildren<Text>().ToList();
        }

        void Update()
        {
            CheckAmount();
        }

        void CheckAmount()
        {
            foreach (var amount in Amounts)
            {
                bool gotEmpty = amount.text == "0";
                //amount.GetComponentInParent<Button>().interactable = !gotEmpty;
            }
        }

        public void SetActiveItem()
        {
            Cake.GetComponent<Outline>().enabled = false;
            Burger.GetComponent<Outline>().enabled = false;
            Donut.GetComponent<Outline>().enabled = false;
            Ice.GetComponent<Outline>().enabled = false;
            Muffin.GetComponent<Outline>().enabled = false;
            Waffle.GetComponent<Outline>().enabled = false;
            
            switch (CurrentFoodType)
            {
                case FoodType.Cake: Current = Cake;     break;
                case FoodType.Hamburger: Current = Burger;   break;
                case FoodType.IceCream: Current = Ice;      break;
                case FoodType.Muffin: Current = Muffin; break;
                case FoodType.Waffle: Current = Waffle;   break;
                case FoodType.Donut: Current = Donut;    break;
            }

            Current.GetComponent<Outline>().enabled = true;
        }

        public void SetFoodType(int foodtype)
        {
            CurrentFoodType = (FoodType) foodtype;
            SetActiveItem();
        }

    }
}
