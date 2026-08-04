using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]

public class SFXPlayer : GenericSingleton<SFXPlayer>
{
    AudioSource audioSource;
    [SerializeField] AudioClip clip;

    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    // Update is called once per frame
    public void PlaySFX()
    {
        audioSource.PlayOneShot(clip);
    }
}
