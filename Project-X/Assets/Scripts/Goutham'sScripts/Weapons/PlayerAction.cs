using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DisallowMultipleComponent]
public class PlayerAction : MonoBehaviour
{
    [SerializeField]private PlayerGunSelector gunSelector;

    private void Update()
    {
        if(Input.GetMouseButton(0))
        {
            gunSelector.activeGun.Shoot();
        }
    }
}
