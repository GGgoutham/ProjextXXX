using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class PlayerAction : MonoBehaviour
{
    [SerializeField]private PlayerGunSelector gunSelector;

    [SerializeField] private bool autoReload = true;

    [SerializeField] private Image crosshair;

    [SerializeField] private Animator playerAnimator;

    [SerializeField] private Transform aimTarget;
    

    //private bool isReloading;


    private void Update()
    {
        if(gunSelector.activeGun!=null)
        {
            gunSelector.activeGun.Tick(Input.GetMouseButton(0));
        }

        if(ShouldAutoReload()||ShouldManualReload())
        {
            gunSelector.activeGun.StartReloading();//plays Reloading Audio
            gunSelector.activeGun.isReloading = true;
            playerAnimator.SetTrigger("Reload");
        }

        UpdateCrossHair();
        gunSelector.gunParent.transform.LookAt(aimTarget);
    }

    public void EndReload()
    {
        Debug.Log("reloaded");
        gunSelector.activeGun.End_Reload();
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

    private void UpdateCrossHair()
    {
        Vector3 gunTipPoint = gunSelector.activeGun.GetRayCastOrigin();
        Vector3 forward;
        if (gunSelector.activeGun.shootConfig.shootType == ShootType.FromGun)
        {
            forward = gunSelector.activeGun.GetGunForward();
        }
        else
        {
            forward = gunSelector.camera.transform.forward;
        }

        Vector3 hitPoint = gunTipPoint + forward * 10;
        if (Physics.Raycast(gunTipPoint, forward, out RaycastHit hit, float.MaxValue, gunSelector.activeGun.shootConfig.hitMask))
        {
            hitPoint = hit.point;
        }

        aimTarget.transform.position = hitPoint;

        if (gunSelector.activeGun.shootConfig.shootType == ShootType.FromGun)
        {
            Vector3 screenSpaceLocation = gunSelector.camera.WorldToScreenPoint(hitPoint);

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)crosshair.transform.parent,
                screenSpaceLocation,
                null,
                out Vector2 localPosition))
            {
                crosshair.rectTransform.anchoredPosition = localPosition;
            }
            else
            {
                crosshair.rectTransform.anchoredPosition = Vector2.zero;
            }
        }
        else
        {
            crosshair.rectTransform.anchoredPosition = Vector2.zero;
        }
    }


}
