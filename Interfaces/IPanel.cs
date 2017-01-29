using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    interface IPanel
    {
        void Start();
        void Update();
        void SetActive(bool active);
    }
}
