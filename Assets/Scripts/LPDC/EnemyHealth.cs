using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace LPDC
{
    public class EnemyHealth : MonoBehaviour
    {
        public int hp = 3;

        public void ApplyDamage(int damageAmount)
        {
            hp = Math.Max(0, hp - damageAmount);
            if (hp == 0)
                Destroy(gameObject);
        }
    }

}