using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Parallax : MonoBehaviour
{

    float length, startposition;
    public float parallaxEffect;
    public GameObject cam;

    // Start is called before the first frame update
    void Start()
    {
        startposition = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;

    }

    // Update is called once per frame
    void Update()
    {
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        float dist = (cam.transform.position.x * parallaxEffect);

        transform.position = new Vector3(startposition + dist, transform.position.y, transform.position.z);

        if (temp > startposition + length) startposition += length;
        else if (temp < startposition - length) startposition -= length;
    }
}
