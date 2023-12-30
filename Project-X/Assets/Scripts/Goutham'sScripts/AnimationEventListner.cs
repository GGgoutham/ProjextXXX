using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventListner : MonoBehaviour
{
   // public delegate void Endreload();
  //  public event Endreload OnReloadComplete;

    public UnityEvent Endreload;

    private void EndReload()
    {
        Debug.Log("called EndReload");
        Endreload?.Invoke();
    }



}
