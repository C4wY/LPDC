using System.Collections;
using System.Collections.Generic;
using LPDC;
using UnityEngine;

public class DummyHealthDisplay : MonoBehaviour
{
    void Update()
    {
        var hp = GetComponentInParent<EnemyHealth>().hp;
        GetComponent<TMPro.TextMeshPro>().text = $"hp: {hp}";
    }
}
