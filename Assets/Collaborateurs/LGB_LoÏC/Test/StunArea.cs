using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StunArea : MonoBehaviour

{
    public UnityEvent onTrigger;
    // Start is called before the first frame update
    void OnTriggerEnter()
    {
        onTrigger.Invoke();

    }



}
