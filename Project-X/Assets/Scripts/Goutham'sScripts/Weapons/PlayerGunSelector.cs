using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DisallowMultipleComponent]
public class PlayerGunSelector : MonoBehaviour
{
    public Camera camera;

    [SerializeField] private GunType gun;
    [SerializeField] public Transform gunParent;
    [SerializeField] private List<GunSO> guns;
    [SerializeField] private PlayerIK playerIK;
    //[SerializeField] private PlayerIK inverseKinematics;

    [Space]
    [Header("Runtime Filled")]
    public GunSO activeGun;
    private void Awake()
    {
        
        playerIK = GetComponent<PlayerIK>();
    }

    private void Start()
    {
        GunSO gun_ = guns.Find(gun_ => gun_.type == gun);

        if (gun_ == null) { Debug.LogError($"No GunSO found for guntype :{gun}"); }

    activeGun = gun_;
        gun_.Spawn(gunParent, this,camera);

       // playerIK.Setup(gunParent);
       
    }

}
