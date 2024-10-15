using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace LPDC
{
    public class MIC_Santé_Joueur : MonoBehaviour
    {

        public int MIC_viemax;
        public float MIC_compteurInvincibilité;

        public int PV = 3;

        void Update()
        {
            if (MIC_compteurInvincibilité > 0)
            {
                MIC_compteurInvincibilité -= Time.deltaTime;

            }
        }
        public void FaireDégâts(int dégâts = 1)
        {
            if (MIC_compteurInvincibilité <= 0)

            {

                Debug.Log("OUCH");
                PV += -dégâts;

                if (PV == 0)
                {
                    Debug.Log("Jeu fini");
                    Debug.Break();
                }
            }
        }
    }
}

