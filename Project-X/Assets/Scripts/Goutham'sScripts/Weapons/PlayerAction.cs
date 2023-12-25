using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DisallowMultipleComponent]
public class PlayerAction : MonoBehaviour
{
    [SerializeField]private PlayerGunSelector gunSelector;

    private void Update()
    {
        if(gunSelector.activeGun!=null)
        {
            gunSelector.activeGun.Tick(Input.GetMouseButton(0));
        }
    }
}
