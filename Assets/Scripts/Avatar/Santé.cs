using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Avatar
{
    public class Santé : MonoBehaviour
    {

        public int viemax;
        public float compteurInvincibilité;

        public int PV = 3;

        void Update()
        {
            if (compteurInvincibilité > 0)
            {
                compteurInvincibilité -= Time.deltaTime;

            }
        }
        public void FaireDégâts(int dégâts = 1)
        {
            if (compteurInvincibilité <= 0)

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

