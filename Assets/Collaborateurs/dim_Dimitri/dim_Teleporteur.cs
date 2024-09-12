using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dim_Teleporteur : MonoBehaviour
{
    public Transform destination;
    private Transform playerPosition;
    private GameObject player;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            // Test : le collider est bien le leader
            var isLeader = LPDC.Avatar.GetLeader() == collider.GetComponentInParent<LPDC.Avatar>();
            if (isLeader)
            {
                player = collider.gameObject;
                playerPosition = player.transform;
                player.SetActive(false);
                playerPosition.position = destination.position;
                player.SetActive(true);
            }
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
