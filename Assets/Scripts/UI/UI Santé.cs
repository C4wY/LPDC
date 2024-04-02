using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class UISanté : MonoBehaviour
{
    public Sprite plein;
    public Sprite vide;

    void Update()
    {
        var avatars = FindObjectsByType<Avatar.Avatar>(FindObjectsSortMode.None);
        var récupleader = avatars.First(a => a.IsLeader);
        var PV = récupleader.Santé.PV;

        for (int i = 0; i < 3; i++)
        {
            var image = transform.GetChild(i).GetComponent<Image>();
            if (i < PV)
            {
                image.sprite = plein;
            }
            else
            {
                image.sprite = vide;
            }
        }
    }
}
