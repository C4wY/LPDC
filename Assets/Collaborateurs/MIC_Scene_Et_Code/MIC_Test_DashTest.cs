using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MIC_Unj_DashTest : MonoBehaviour
{
    public float moveSpeed;
    Avatar.Avatar avatar;
    public Vector2 moveInput;

    public float activemoveSpeed;
    public float dashSpeed;

    public float dashLenght = .5f, dashCooldown = 1f;

    private float dashCounter;
    public float dashCoolCounter;

    void OnEnable()
    {
        avatar = GetComponent<Avatar.Avatar>();
    }
    // Start is called before the first frame update
    void Start()
    {
        activemoveSpeed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        moveInput.Normalize();

        avatar.Rigidbody.velocity = moveInput * activemoveSpeed;

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (dashCoolCounter <= 0 && dashCounter <= 0)
            {
                activemoveSpeed = dashSpeed;
                dashCoolCounter = dashLenght;
            }
        }

        if (dashCoolCounter > 0)
        {
            dashCoolCounter -= Time.deltaTime;
        }

        if (dashCoolCounter < 0)
        {
            moveSpeed = 5;
            activemoveSpeed = 5;
        }
    }
}
