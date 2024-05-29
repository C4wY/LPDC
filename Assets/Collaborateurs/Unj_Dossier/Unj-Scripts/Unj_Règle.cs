using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unj_Règle : MonoBehaviour
{
    public GameObject Cube;

    // Update is called once per frame
    void Update()
    {
        var p1 = gameObject.transform.TransformPoint(0, 0, 0);
        var p2 = gameObject.transform.TransformPoint(1, 1, 0);
        var w = p2.x - p1.x;
        var h = p2.y - p1.y;
    }
}
