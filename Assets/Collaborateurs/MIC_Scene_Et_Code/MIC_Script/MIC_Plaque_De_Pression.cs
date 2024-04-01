using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MIC_Plaque_De_Pression : MonoBehaviour

{
    public GameObject leverText;
    public AudioClip activationSound;
    public AudioClip deactivationSound;
    public float activationDuration = 3f;
        private bool isPlayerInRange = false;
    private bool PlaqueActivated = false;
    public float interactionDistance = 3f;
        public AudioSource audioSource;
        public GameObject wallToDisappear;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
      private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            leverText.SetActive(true);
             {
                audioSource.PlayOneShot(activationSound);
            }
                    if (wallToDisappear != null)
        {
            wallToDisappear.SetActive(false);
        }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(DeactivateAfterDelay());
          
        }
    }
        private IEnumerator DeactivateAfterDelay()
    {
        yield return new WaitForSeconds(activationDuration);
        isPlayerInRange = false;
        leverText.SetActive(false);
         if (deactivationSound != null)
        {
            audioSource.PlayOneShot(deactivationSound);
        }
                if (wallToDisappear != null)
        {
            wallToDisappear.SetActive(true);
        }

    }

}

