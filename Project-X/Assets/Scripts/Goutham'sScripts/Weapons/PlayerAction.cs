using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DisallowMultipleComponent]
public class PlayerAction : MonoBehaviour
{
    [SerializeField]private PlayerGunSelector gunSelector;

    [SerializeField]private bool autoReload = true;

    [SerializeField] private Animator playerAnimator;

    //private bool isReloading;


    private void Update()
    {
        if(gunSelector.activeGun!=null)
        {
            gunSelector.activeGun.Tick(Input.GetMouseButton(0));
        }

        if(ShouldAutoReload()||ShouldManualReload())
        {
            gunSelector.activeGun.StartReloading();
            gunSelector.activeGun.isReloading = true;
            playerAnimator.SetTrigger("Reload");
        }
    }

    private void EndReload()
    {
        Debug.Log("reloaded");
        gunSelector.activeGun.EndReload();
        gunSelector.activeGun.isReloading = false;
    }

    private bool ShouldAutoReload()
    {   
        return !gunSelector.activeGun.isReloading && autoReload && 
            gunSelector.activeGun.ammoConfig.currentClipAmmo == 0 && 
            gunSelector.activeGun.CanReload();
    }
    private bool ShouldManualReload()
    {
        return !gunSelector.activeGun.isReloading && Input.GetKeyDown(KeyCode.R) && gunSelector.activeGun.CanReload();
    }


}
