using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngineInternal;

[CreateAssetMenu(fileName ="Gun",menuName ="Guns/gun",order =0)]
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

    private MonoBehaviour activeMonoBehaviour;
    private GameObject model;
    private float lastShootTime;
    private ParticleSystem shootSystem;
    private ObjectPool<TrailRenderer> trailPool;

    private void Awake()
    {
        //firePoint = model.transform;
    }


    public void Spawn(Transform parent, MonoBehaviour activeMonoBehaviour)
    {

        this.activeMonoBehaviour = activeMonoBehaviour;
        lastShootTime = 0;
        trailPool = new ObjectPool<TrailRenderer>(CreateTrail);

        model = Instantiate(modelPrefab);
        model.transform.SetParent(parent, false);
        model.transform.localPosition = spawnPoint;
        model.transform.localRotation = Quaternion.Euler(spawnRotation);
        
        shootSystem = model.GetComponentInChildren<ParticleSystem>();


    }

    public void Shoot()
    {
        if (Time.time>shootConfig.fireRate+lastShootTime)
        {
            lastShootTime=Time.time;
            shootSystem.Play();
            Vector3 shootDir = Camera.main.transform.forward + new Vector3(
                Random.Range(-shootConfig.spread.x, shootConfig.spread.x),
                Random.Range(-shootConfig.spread.y, shootConfig.spread.y),
                Random.Range(-shootConfig.spread.z, shootConfig.spread.z));

            shootDir.Normalize();

            if (Physics.Raycast(Camera.main.transform.position,shootDir,out RaycastHit hit,float.MaxValue,shootConfig.hitMask))
            {
                activeMonoBehaviour.StartCoroutine(PlayTrail(model.transform.position,
                    hit.point,
                    hit));

                Debug.Log("shooting target");

            }
            else
            {
                activeMonoBehaviour.StartCoroutine(PlayTrail(Camera.main.transform.position, 
                    shootSystem.transform.position + (shootDir*trailConfig.missDistance),
                    new RaycastHit()));


                Debug.Log("shooting stuff");

            }

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
            SurfaceManager.Instance.HandleImpact(Hit.transform.gameObject, endPoint, Hit.normal, impactType, 0);

        }

        yield return new WaitForSeconds(trailConfig.duration);
        yield return null;
        instance.emitting= false;
        instance.gameObject.SetActive(false);
        trailPool.Release(instance);


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

    



}
