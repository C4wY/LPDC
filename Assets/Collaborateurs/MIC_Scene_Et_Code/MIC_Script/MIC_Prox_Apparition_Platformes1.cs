using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MIC_Prox_Apparition_Platformes : MonoBehaviour
{
    public KeyCode activationKey = KeyCode.F;
    public float interactionDistance = 3f;
    public GameObject leverText;
    public GameObject[] platformsToActivate;
    public AudioClip leverSound;
    public AudioSource audioSource;
    public Transform leverHandle;
    public Vector3 leverActivatedRotation;

    private bool isPlayerInRange = false;
    private bool leverActivated = false;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(activationKey))
        {
            ActivateLever();
        }
    }

    HashSet<Collider> colliders = new HashSet<Collider>();
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            colliders.Add(other);
            if (colliders.Count == 1)
            {
                isPlayerInRange = true;
                leverText.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            colliders.Remove(other);
            if (colliders.Count == 0)
            {
                isPlayerInRange = false;
                leverText.SetActive(false);
            }
        }
    }

    void ActivateLever()
    {
        if (!leverActivated)
        {
            leverActivated = true;

            foreach (GameObject platform in platformsToActivate)
                platform.SetActive(true);

            if (audioSource && leverSound)
                audioSource.PlayOneShot(leverSound);

            if (leverHandle != null)
            {
                leverHandle.rotation = Quaternion.Euler(leverActivatedRotation);
            }
        }
    }
}
