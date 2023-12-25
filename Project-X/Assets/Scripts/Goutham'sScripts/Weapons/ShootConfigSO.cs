using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Shoot Config", menuName = "Guns/Shoot Config", order = 2)]
public class ShootConfigSO : ScriptableObject, System.ICloneable
{
    public bool isHitscan = true;
   // public Bullet BulletPrefab;
    public float bulletSpawnForce = 100;
    public LayerMask hitMask;
    public float fireRate = 0.25f;
    public BulletSpreadType spreadType = BulletSpreadType.Simple;
    public float recoilRecoverySpeed = 1f;
    public float maxSpreadTime = 1f;
    public float bulletWeight = 0.1f;

   // public ShootType ShootType = ShootType.FromGun;


    [Header("Simple Spread")]
    public Vector3 spread = new Vector3(0.1f, 0.1f, 0.1f);
    [Header("Texture-Based Spread")]

    [Range(0.001f, 5f)]
    public float spreadMultiplier = 0.1f;

    public Texture2D spreadTexture;


    public Vector3 GetSpread(float ShootTime = 0)
    {
        Vector3 spread_ = Vector3.zero;

        if (spreadType == BulletSpreadType.Simple)
        {
            spread_ = Vector3.Lerp(
                Vector3.zero,
                new Vector3(
                    Random.Range(-spread.x, spread.x),
                    Random.Range(-spread.y, spread.y),
                    Random.Range(-spread.z, spread.z)
                ),
                Mathf.Clamp01(ShootTime / maxSpreadTime)
            );
        }
        else if (spreadType == BulletSpreadType.TextureBased)
        {
            spread = GetTextureDirection(ShootTime);
            spread *= spreadMultiplier;
        }

        return spread_;
    }

      
    private Vector2 GetTextureDirection(float ShootTime)
    {
        Vector2 halfSize = new Vector2(spreadTexture.width / 2f, spreadTexture.height / 2f);

        int halfSquareExtents = Mathf.CeilToInt(Mathf.Lerp(0.01f, halfSize.x, Mathf.Clamp01(ShootTime / maxSpreadTime)));

        int minX = Mathf.FloorToInt(halfSize.x) - halfSquareExtents;
        int minY = Mathf.FloorToInt(halfSize.y) - halfSquareExtents;

        Color[] sampleColors = spreadTexture.GetPixels(
            minX,
            minY,
            halfSquareExtents * 2,
            halfSquareExtents * 2
        );

        float[] colorsAsGrey = System.Array.ConvertAll(sampleColors, (color) => color.grayscale);
        float totalGreyValue = colorsAsGrey.Sum();

        float grey = Random.Range(0, totalGreyValue);
        int i = 0;
        for (; i < colorsAsGrey.Length; i++)
        {
            grey -= colorsAsGrey[i];
            if (grey <= 0)
            {
                break;
            }
        }

        int x = minX + i % (halfSquareExtents * 2);
        int y = minY + i / (halfSquareExtents * 2);

        Vector2 targetPosition = new Vector2(x, y);

        Vector2 direction = (targetPosition - new Vector2(halfSize.x, halfSize.y)) / halfSize.x;

        return direction;
    }

    public object Clone()
    {
        ShootConfigSO config = CreateInstance<ShootConfigSO>();

        //Utilities.CopyValues(this, config);

        return config;
    }
}
