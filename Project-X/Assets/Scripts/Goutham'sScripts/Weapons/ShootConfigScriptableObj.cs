using UnityEngine;

[CreateAssetMenu(fileName ="shoot config",menuName ="Guns/Shoot Config",order =2)]
public class ShootConfigScriptableObj : ScriptableObject
{
    public LayerMask hitMask;
    public Vector3 spread= new Vector3(0.1f,0.1f,0.1f);
    public float fireRate = 0.25f;

}
