using Assets.Scripts.Manager;
using System;
using System.Collections.Generic;
using Assets.Scripts.Player;
using Random = System.Random;

namespace Assets.Scripts.Norms
{
    [Serializable]
    public class NormManager
    {
        public List<Norm> Norms = new List<Norm>();
        
        public NormManager()
        {
            foreach (var n in EnvironmentManager.Instance.Settings.Norms)
            {
                Norms.Add(n);
            }
        }
    }
}
