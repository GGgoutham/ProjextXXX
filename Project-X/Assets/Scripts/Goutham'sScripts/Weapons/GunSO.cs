
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngineInternal;

[CreateAssetMenu(fileName ="Gun", menuName="Guns/gun",order =0)]
public class GunSO : ScriptableObject
{
    public ImpactType impactType;
    public GunType type;
    public string Name;
    public GameObject modelPrefab;
    public Vector3 spawnPoint;
    public Vector3 spawnRotation;
    //public Transform firePoint;

    public ShootConfigSO shootConfig;
    public TrailConfigSO trailConfig;
    public DamageConfigSO damageConfig;
    public AmmoConfigSO ammoConfig;
    public AudioConfigSO audioConfig;


    private MonoBehaviour activeMonoBehaviour;
    private GameObject model;
    private AudioSource shootingAudioSource;
    private Camera activeCamera;

    private float lastShootTime;
    private float initialClickTime;
    private float stopShootTime;
    private bool lastFrameWantedToShoot;

    public bool isReloading=false;


    private ParticleSystem shootSystem;
    private ObjectPool<TrailRenderer> trailPool;

    private void Awake()
    {

    }


    public void Spawn(Transform parent, MonoBehaviour activeMonoBehaviour, Camera activeCamera = null)
    {

        this.activeMonoBehaviour = activeMonoBehaviour;
        this.activeCamera = activeCamera;

        lastShootTime = 0;
        trailPool = new ObjectPool<TrailRenderer>(CreateTrail);

        model = Instantiate(modelPrefab);
        model.transform.SetParent(parent, false);
        model.transform.localPosition = spawnPoint;
        model.transform.localRotation = Quaternion.Euler(spawnRotation);
        
        shootSystem = model.GetComponentInChildren<ParticleSystem>();
        shootingAudioSource = model.GetComponent<AudioSource>();


    }

    public void TryToShoot()
    {
        if (Time.time - lastShootTime - shootConfig.fireRate > Time.deltaTime)
        {
            float lastDuration = Mathf.Clamp(0, (stopShootTime - initialClickTime), shootConfig.maxSpreadTime);

            float lerpTime = (shootConfig.recoilRecoverySpeed - (Time.time - stopShootTime)) / shootConfig.recoilRecoverySpeed;

            initialClickTime = Time.time- Mathf.Lerp(0, lastDuration,Mathf.Clamp01(lerpTime));
        }
        if (Time.time>shootConfig.fireRate+lastShootTime)
        {
            lastShootTime=Time.time;
            if (ammoConfig.currentClipAmmo == 0)
            {
                audioConfig.PlayOutOfAmmoClip(shootingAudioSource);
                return;
            } 
            shootSystem.Play();
            audioConfig.PlayShootingCLip(shootingAudioSource, ammoConfig.currentAmmo == 1);

            Vector3 spreadAmmount = shootConfig.GetSpread(Time.time-initialClickTime);
            model.transform.forward += model.transform.TransformDirection(spreadAmmount);//moves model according to recoil

            Vector3 shootDir = Vector3.zero ;

            if(shootConfig.shootType == ShootType.FromGun)
            {
                shootDir = shootSystem.transform.forward;
            }
            else
            {
                shootDir = activeCamera.transform.forward + activeCamera.transform.TransformDirection(shootDir);
            }

            ammoConfig.currentClipAmmo--;

            if (Physics.Raycast(GetRayCastOrigin(),shootDir,out RaycastHit hit,float.MaxValue,shootConfig.hitMask.value))
            {
                activeMonoBehaviour.StartCoroutine(PlayTrail(shootSystem.transform.position,
                    hit.point,
                    hit));

                Debug.Log("shooting target");

            }
            else
            {
                activeMonoBehaviour.StartCoroutine(PlayTrail(shootSystem.transform.position, 
                    shootSystem.transform.position + (shootDir*trailConfig.missDistance),
                    new RaycastHit()));


                Debug.Log("shooting stuff");

            }

        }

    }

    public void UpdateCamera(Camera activeCamera)
    {
        this.activeCamera = activeCamera;
    }

    public void Tick(bool wantsToShoot)
    {
        
        model.transform.localRotation = Quaternion.Lerp(
            model.transform.localRotation,
            Quaternion.Euler(spawnRotation),
            Time.deltaTime*shootConfig.recoilRecoverySpeed);

        if (wantsToShoot)
        {
            lastFrameWantedToShoot = true;

            if (!isReloading)
            {
                TryToShoot();
               
            }
        }else if (!wantsToShoot && lastFrameWantedToShoot)
        {
            stopShootTime = Time.time;
            lastFrameWantedToShoot = false;
        }

    } 

    private IEnumerator PlayTrail(Vector3 startPoint, Vector3 endPoint,RaycastHit Hit)
    {
        TrailRenderer instance = trailPool.Get();
        instance.gameObject.SetActive(true);
        instance.transform.position = startPoint;
        yield return null;//waiting 1 sec to avoid position from last fram carrying over if reused

        instance.emitting = true;

        float distance = Vector3.Distance(startPoint, endPoint);
        float remainingDistance = distance;
        while(remainingDistance > 0)
        {
            instance.transform.position = Vector3.Lerp(
                startPoint,endPoint,Mathf.Clamp01(1-(remainingDistance/distance)));

            remainingDistance -= trailConfig.simulationSpeed*Time.deltaTime;
            yield return null;
        }

        instance.transform.position = endPoint;

        if (Hit.collider!=null)
        {

            // SurfaceManager.Instance.HandleImpact(Hit.transform.gameObject, endPoint, Hit.normal, impactType, 0);  //for later

            if (Hit.collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damageConfig.GetDamage(distance));
            }

        }

        yield return new WaitForSeconds(trailConfig.duration);
        yield return null;
        instance.emitting= false;
        instance.gameObject.SetActive(false);
        trailPool.Release(instance);


    }

    public Vector3 GetRayCastOrigin()
    {
        Vector3 origin = shootSystem.transform.position;

        if (shootConfig.shootType== ShootType.FromCam)
        {
            origin = activeCamera.transform.position + activeCamera.transform.forward * Vector3.Distance(
                activeCamera.transform.position, shootSystem.transform.position
                );
        }

        return origin;
    }
    public Vector3 GetGunForward()
    {
        return model.transform.forward;
    }


    private TrailRenderer CreateTrail()
    {
        GameObject instance = new GameObject("Bullet Trail");
        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        trail.colorGradient = trailConfig.colour;
        trail.material = trailConfig.material;
        trail.widthCurve = trailConfig.widthCurve;
        trail.time = trailConfig.duration;
        trail.minVertexDistance = trailConfig.minVertexDistance;

        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
        
    }

    public bool CanReload()
    {
        return ammoConfig.CanReload();
    }

    public void End_Reload()
    {
        ammoConfig.Reload();
    }

    public void StartReloading()
    {
        audioConfig.PlayReloadCLip(shootingAudioSource);
    }





}
