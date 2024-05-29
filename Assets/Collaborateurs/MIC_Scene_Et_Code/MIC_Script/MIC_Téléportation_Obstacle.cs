using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MIC_Téléportation_Obstacle : MonoBehaviour
{
    public Transform teleportReference;
    public AudioClip teleportSound;

    // private bool hasTeleported = false;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = teleportReference.position;

            if (teleportSound != null)
            {
                audioSource.PlayOneShot(teleportSound);
            }
            // hasTeleported = true;
        }
    }

}
