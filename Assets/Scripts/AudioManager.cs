using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public float BGMFadeTimer = 2f;

    public Sound[] sounds;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Stop();
    }

    public void FadeoutBGM(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        StartCoroutine(FadeOutBGM(name));
    }

    IEnumerator FadeOutBGM(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.volume = s.volume;
        float currentTime = 0;
        while (s.source.volume > 0)
        {
            currentTime += Time.deltaTime;
            s.source.volume = Mathf.Lerp(s.volume, 0, (currentTime / BGMFadeTimer));
            yield return null;
        }
        s.source.volume = 0;
        s.source.Stop();
        StopCoroutine(FadeOutBGM(name));
    }

    public void FadeinBGM(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        StartCoroutine(FadeInBGM(name));
    }

    IEnumerator FadeInBGM(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
        s.source.volume = 0;
        float currentTime = 0;
        while (s.source.volume < s.volume)
        {
            currentTime += Time.deltaTime;
            s.source.volume = Mathf.Lerp(0, s.volume, (currentTime / BGMFadeTimer));
            yield return null;
        }
        s.source.volume = s.volume;
        StopCoroutine(FadeInBGM(name));
    }
}