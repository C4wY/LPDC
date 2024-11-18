using System.Collections;
using System.Collections.Generic;
using Ink.Parsed;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public bool isEnabled = false;
    public float coolDown = 3f;
    public int damage = 1;

    float attackTime = 0;

    public bool CanAttack()
    {
        return isEnabled && attackTime <= 0;
    }

    void DoAttack(LPDC.Avatar avatar)
    {
        avatar.Santé.FaireDégâts(damage);
        attackTime = coolDown;
    }

    void OnTriggerStay(Collider other)
    {
        var avatar = other.attachedRigidbody.GetComponent<LPDC.Avatar>();
        if (avatar != null)
        {
            if (avatar.IsLeader)
            {
                if (CanAttack())
                {
                    DoAttack(avatar);
                }
            }
        }
    }

    void Update()
    {
        attackTime += -Time.deltaTime;
    }
}
