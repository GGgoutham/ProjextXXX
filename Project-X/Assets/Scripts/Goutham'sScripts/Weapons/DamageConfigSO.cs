using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;


[CreateAssetMenu(fileName ="DamageCOnfig",menuName ="Guns/Damage Config",order =1)]
public class DamageConfigSO : ScriptableObject
{
    public MinMaxCurve DamageCurve;

    private void Reset()//called on object creation
    {
        DamageCurve.mode = ParticleSystemCurveMode.Curve;
    }

    public int GetDamage(float Distance =0 )
    {
        return Mathf.CeilToInt( DamageCurve.Evaluate( Distance ,Random.value) );
    }



}
