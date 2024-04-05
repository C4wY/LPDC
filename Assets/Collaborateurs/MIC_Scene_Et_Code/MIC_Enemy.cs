using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MIC_Enemy : MonoBehaviour
{
    public int dégâts = 1;

    private bool isPlayerInRange = false;

        public int PV = 3;
        public KeyCode activationKey = KeyCode.A;

        public void FaireDégâts(int dégâts = 1)
        {
            PV += -dégâts;

            if (PV == 0)
            {
                Debug.Log("cassé");
                Debug.Break();
            }
        }
     void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Joueur");
            isPlayerInRange = true;
           if (Input.GetKeyDown(activationKey))
        {
            FaireDégâts(dégâts);
            Debug.Log("aaaaaaa");
        }
        }

           
    }
     private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    
}

