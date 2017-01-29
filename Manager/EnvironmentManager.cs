
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Ant;
using Assets.Scripts.Jobs;
using Assets.Scripts.Norms;
using Assets.Scripts.Player;
using Assets.Scripts.SettingClasses;
using UnityEngine;

namespace Assets.Scripts.Manager
{
    public class EnvironmentManager
    {
        
        private static EnvironmentManager _instance;        
        private List<Contract> _contracts;
        private Setting _setting;
        private SpawnManager _spawnManager;
        private Controller _controller;
        private UserInterfaceManager _userInterfaceManager;
        private NormManager _normManager;
        private NewsManager _newsManager;
        private AntStatsObserver _observer;
        private GameManager _gameManager;
        private AntEater _player;
        private FlowManager _flowManager;
        private PlayerStats _stats;


        private EnvironmentManager()
        {
            CurrentSetting = "settings.json";
        }

        public static EnvironmentManager Instance
        {
            get { return _instance ?? (_instance = new EnvironmentManager()); }   
        }

        public string CurrentSetting { get; set; }

        public AntStatsObserver Observer
        {
            get { return _observer ?? GameObject.FindObjectOfType<AntStatsObserver>();  }
        }

        public List<Job> Jobs {
            get { return Controller.AntContainer.GetComponentsInChildren<Job>().ToList(); }
        }

        public List<AntAI> Ants
        {
            get
            {
                var ants = Controller.AntContainer.GetComponentsInChildren<AntAI>().ToList();
                return ants;
            }
        }

        public Controller Controller
        {
			get { return _controller ?? (_controller = GameObject.FindObjectOfType<Controller>()); }
        }

        public List<Contract> Contracts
        {
            get
            {
                var contracts = Ants.SelectMany(n => n.Agent.Contracts).ToList();
                return contracts;
            } 
        }

        public Setting Settings
        {
            get { return _setting ?? (this._setting = Serializer.LoadSettings(CurrentSetting)); }
            set { this._setting = value; Serializer.SaveToJson(value, CurrentSetting); }
        }

        public SpawnManager SpawnManager
        {
			get { return _spawnManager ?? (_spawnManager = GameObject.FindObjectOfType<SpawnManager>()); }
        }

        public UserInterfaceManager UiManager
        {
			get { return _userInterfaceManager ?? (_userInterfaceManager = GameObject.FindObjectOfType<UserInterfaceManager>()); }
        }

        public NormManager NormManager
        {
            get { return _normManager ?? (_normManager = new NormManager()); }
        }

        public NewsManager NewsManager
        {
            get { return _newsManager ?? (_newsManager = new NewsManager()); }
        }

        public GameManager GameManager
        {
            get { return _gameManager ?? (_gameManager = GameObject.FindObjectOfType<GameManager>()); }
        }

        public AntEater Player
        {
            get { return _player ?? (_player = GameObject.FindObjectOfType<AntEater>()); }
        }

        public FlowManager FlowManager
        {
            get { return _flowManager ?? (_flowManager = GameObject.FindObjectOfType<FlowManager>()); }
        }

        public void Reset()
        {
            _instance = null;
        }
    }


}
