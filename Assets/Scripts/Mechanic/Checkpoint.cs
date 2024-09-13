using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CheckPoint : MonoBehaviour
{
    public int checkPointOrder = -1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnEnable()
    {
        if (checkPointOrder == -1)
        {
            var instances = FindObjectsByType<CheckPoint>(FindObjectsSortMode.None);
            checkPointOrder = instances.Length;
        }

        gameObject.name = $"CheckPoint-{checkPointOrder}";
    }
}
