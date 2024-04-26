using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MIC_SoundManager : MonoBehaviour
{
    public static MIC_SoundManager instance;

    private AudioSource[] allAudioSources;
    public AudioSource audioSourcePoursuite; //Référence à la musique de pouruite

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
    }

    public void StopAllSounds()
    {
        foreach (AudioSource source in allAudioSources)
        {
            source.Stop();

            if (source == audioSourcePoursuite)
            {
                // Arrêter la musique de poursuite
                source.Stop();
            }
        }
    }
}

