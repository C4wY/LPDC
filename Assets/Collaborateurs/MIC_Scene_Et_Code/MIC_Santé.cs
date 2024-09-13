using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LPDC
{
    public class MIC_Santé : MonoBehaviour
    {
        public int PV = 3;

        public void FaireDégâts(int dégâts = 1)
        {
            PV += -dégâts;

            if (PV == 0)
            {
                Debug.Log("Jeu fini");
                Debug.Break();
            }
        }
    }
}

