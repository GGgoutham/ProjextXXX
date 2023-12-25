using UnityEngine;

[CreateAssetMenu(fileName ="Trail config",menuName ="Guns/Trail Config",order =4)]
public class TrailConfigSO : ScriptableObject
{
    public Material material;
    public AnimationCurve widthCurve;
    public float duration = 0.5f;
    public float minVertexDistance = 0.1f;
    public Gradient colour;

    public float missDistance = 100f;
    public float simulationSpeed = 100f;

}
