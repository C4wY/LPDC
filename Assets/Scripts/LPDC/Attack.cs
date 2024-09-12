using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LPDC
{

    public class Attack : MonoBehaviour
    {
        Avatar avatar;
        Avatar Avatar =>
            avatar != null ? avatar : avatar = GetComponentInParent<Avatar>();

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            var direction = Avatar.Move.FacingDirection;
            transform.localScale = new Vector3(direction, 1, 1);
        }
    }

}