using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doorscript : MonoBehaviour
{
    public Animator Actualdoor;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Actualdoor.ResetTrigger("Close");
            Actualdoor.SetTrigger("Open");
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Actualdoor.ResetTrigger("Open");
            Actualdoor.SetTrigger("Close");
        }
    }
}
