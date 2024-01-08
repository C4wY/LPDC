using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeManager : MonoBehaviour
{

    int life;
    public int Life
    {
        get { return life; }
        set
        {
            if (life > 0 && value <= 0)
            {
                Death();
            }
            else
            {

                life = value;
            }

        }
    }

    public void Death()
    {
        life = 0;
    }

    public bool IsDead
    {
        get { return life == 0; }
    }
}
