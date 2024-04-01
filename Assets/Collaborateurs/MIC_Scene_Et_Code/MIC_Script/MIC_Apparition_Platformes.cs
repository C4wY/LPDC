using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MIC_Apparition_Platformes : MonoBehaviour


{


   public GameObject[] platformsToActivate;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Untagged"))
        {
            ActivatePlatforms();
        }
    }

    private void ActivatePlatforms()
    {
        foreach (GameObject platform in platformsToActivate)
        {
            platform.SetActive(true);
        }
    }
}
