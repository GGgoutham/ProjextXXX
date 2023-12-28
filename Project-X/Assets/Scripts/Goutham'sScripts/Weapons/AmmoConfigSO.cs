using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName ="Ammo Config",menuName ="Guns/Ammo Config",order =3)]
public class AmmoConfigSO : ScriptableObject
{
    public int maxAmmo = 120;
    public int ClipSize = 30;

    public int currentAmmo = 120;
    public int currentClipAmmo = 30;

    

    public void Reload()
    {
        int maxReloadAmmount = Mathf.Min(ClipSize, currentAmmo);
        int availableBulletInCurrentClip = ClipSize-currentClipAmmo;
        int reloadAmmount = Mathf.Min(maxReloadAmmount, availableBulletInCurrentClip);

        currentClipAmmo = currentClipAmmo + reloadAmmount;
        currentAmmo-= reloadAmmount;
    }

    public bool CanReload()
    {
        return currentClipAmmo<ClipSize && currentAmmo > 0;
    }

}
