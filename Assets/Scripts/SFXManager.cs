using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour
{
    [System.Serializable]
    public struct ClipKeyPair
    {
        public string name;
        public AudioClip clip; 
    }

    public static SFXManager instance { get; private set; }

    public List<ClipKeyPair> clips = new List<ClipKeyPair>();

    private AudioSource source;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Debug.LogWarning("Multiple sfx managers in the scene");
            return;
        }

        source = GetComponent<AudioSource>();
        source.loop = false;
        source.playOnAwake = false;
    }

    public static void PlayClip(AudioClip clip, float volume = 1, float pitch = 1)
    {
        if(!instance)
        {
            print("No instance");
            return;
        }

        instance.source.Stop();
        instance.source.time = 0;
        instance.source.pitch = pitch;
        instance.source.volume = volume;
        instance.source.clip = clip;
        instance.source.Play();
    }

    public static void PlayClip(string clipName, float volume = 1, float pitch = 1)
    {
        if(!instance)
        {
            print("No instance");
            return;
        }

        var index = instance.clips.FindIndex(x => x.name == clipName);
        if(index == -1)
        {
            Debug.LogWarning("No clip " + clipName + " found");
            return;
        }

        instance.source.Stop();
        instance.source.time = 0;
        instance.source.pitch = pitch;
        instance.source.volume = volume;
        instance.source.clip = instance.clips[index].clip;
        instance.source.Play();
    }
}