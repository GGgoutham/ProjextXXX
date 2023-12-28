using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="Audio Config",menuName ="Guns/Audio Config",order = 5)]
public class AudioConfigSO : ScriptableObject
{
    [Range(0,1)]
    public float volume = 1f;

    public AudioClip[] fireClips;
    public AudioClip emptyClip;
    public AudioClip reloadClip;
    public AudioClip LastBulletClip;


    public void PlayShootingCLip(AudioSource audioSource, bool isLastBullet = false)
    {
        if (isLastBullet && LastBulletClip != null)
        {
            audioSource.PlayOneShot(LastBulletClip, volume);
        }
        else
        {
            audioSource.PlayOneShot(fireClips[Random.Range(0,fireClips.Length)],volume);
        }

    }

    public void PlayOutOfAmmoClip(AudioSource audioSource)
    {
        if(emptyClip!= null)
        {
            audioSource.PlayOneShot(emptyClip, volume);
        }
    }

    public void PlayReloadCLip(AudioSource audioSource)
    {
        audioSource.PlayOneShot(reloadClip, volume);
    }


}
