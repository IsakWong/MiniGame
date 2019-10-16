using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundObject : ManagedObject
{

    private AudioSource audioSource;
    
    protected void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();

    }
    public void SetData(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
        Invoke("Recycle",clip.length + 0.2f);
    }
    // Update is called once per frame
}
