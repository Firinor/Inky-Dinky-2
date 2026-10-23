using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct ClipSettings
{
    public AudioClip[] Clips;
    public AudioClip Clip
    {
        get
        {
            if (Clips is null || Clips.Length <= 0)
                return null;
            
            return Clips[Random.Range(0, Clips.Length)];
        }
    }

    [Range(0,1)]
    public float Volume;

    public void Play(AudioSource source)
    {
        AudioClip clip = Clip;
        if(clip is null) return;
        if(source is null) return;
        
        source.PlayOneShot(clip, Volume);
    }
}