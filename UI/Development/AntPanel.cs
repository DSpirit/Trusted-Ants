using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Ant;
using Assets.Scripts.Ant.AgentTypes;
using Assets.Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Development
{
    public class AntPanel : MonoBehaviour, IPanel
    {
        public Dropdown AntTypes;
        public Dropdown Groups;
        public InputField Amount;
        public InputField AtTick;



        public void Start()
        {
            AntTypes.options.Clear();
            Groups.options.Clear();

            foreach (AntType type in Enum.GetValues(typeof (AntType)))
            {
                AntTypes.options.Add(new Dropdown.OptionData(type.ToString()));
            }

            foreach (Groups value in Enum.GetValues(typeof(Groups)))
            {
                Groups.options.Add(new Dropdown.OptionData(value.ToString()));
            }

            AntTypes.captionText.text = AntTypes.options[0].text;
            Groups.captionText.text = Groups.options[0].text;
            AntTypes.value = 0;
            Groups.value = 0;
        }

        public void Update()
        {
            
        }

        public void SetActive(bool active)
        {
            this.gameObject.SetActive(active);
        }

        public void CreateSpawnTask()
        {
            var type = (AntType) AntTypes.value;
            var group = (Groups) Groups.value;
            var amount = int.Parse(Amount.text);
            int atSecond;
            if (int.TryParse(AtTick.text, out atSecond))
            {
                var secondsLeft = (TimeSpan.FromSeconds(atSecond) - TimeSpan.FromSeconds(Time.timeSinceLevelLoad)).Seconds;
                secondsLeft = secondsLeft < 0 ? 0 : secondsLeft;
                StartCoroutine_Auto(SpawnEnumerator(type, group, amount, secondsLeft));
            }
            else
            {
                Spawn(type, group, amount);
            }
        }

        public IEnumerator SpawnEnumerator(AntType type, Groups group, int amount, int seconds)
        {
            yield return new WaitForSeconds(seconds);
            Spawn(type, group, amount);
        }

        public void ToggleStates(bool active)
        {
            foreach (var ant in EnvironmentManager.Instance.Ants)
            {
                ant.StateBubble.gameObject.SetActive(active);
            }
        }

        public void Spawn(AntType type, Groups group, int amount)
        {
            var factory = new AgentFactory();
            for (int i = 0; i < amount; i++)
                {
                    GameObject antGo = (GameObject) Instantiate(EnvironmentManager.Instance.Controller.AntPrefab, GlobalFunctions.RandomPointOnNavmesh(100), Quaternion.identity);
                    var agent = factory.CreateAgent(group, type);
                    antGo.GetComponent<AntAI>().Agent = agent;
                    agent.Ant = antGo.GetComponent<AntAI>();
                    antGo.transform.SetParent(EnvironmentManager.Instance.Controller.AntContainer.transform);
            }
        }
    }
}
