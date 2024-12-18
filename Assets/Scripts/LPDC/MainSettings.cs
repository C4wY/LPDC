using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LPDC
{
    [CreateAssetMenu(fileName = "MainSettings", menuName = "LPDC/MainSettings", order = 1)]
    public class MainSettings : ScriptableObject
    {
        public static MainSettings Instance { get; private set; }

        public GameObject gameOverScreen;
        public GameObject cheatScreen;

        MainSettings()
        {
            Instance = this;
        }
    }
}
