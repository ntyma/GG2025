using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    private Coroutine introLoopCoroutine;

    void Awake()
    {
        // ensures only one instance of AudioManager game object
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject); // persist upon switching scenes

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null) // don't try to play sound that's not there
        {
            UnityEngine.Debug.Log("Sound: " + name + " not found");
            return;
        }

        s.source.Play();
    }

    public void PlayIntroThenLoop(string introName, string loopName)
    {
        if (introLoopCoroutine != null) // stop old one if already running
            StopCoroutine(introLoopCoroutine);

        introLoopCoroutine = StartCoroutine(PlayIntroThenLoopCoroutine(introName, loopName));
    }

    private IEnumerator PlayIntroThenLoopCoroutine(string introName, string loopName)
    {
        Play(introName);

        Sound intro = Array.Find(sounds, sound => sound.name == introName);
        if (intro == null) yield break;

        yield return new WaitForSeconds(intro.clip.length);

        Play(loopName);
    }

    // stop everything including coroutines
    public void StopIntroThenLoop(string introName, string loopName)
    {
        if (introLoopCoroutine != null)
        {
            StopCoroutine(introLoopCoroutine);
            introLoopCoroutine = null;
        }

        Stop(introName);
        Stop(loopName);
    }

    public void Pause(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null) // don't try to pause sound that's not there
        {
            UnityEngine.Debug.Log("Sound: " + name + " not found");
            return;
        }

        s.source.Pause();
    }

    public void Unpause(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null) // don't try to pause sound that's not there
        {
            UnityEngine.Debug.Log("Sound: " + name + " not found");
            return;
        }

        s.source.UnPause();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null) // don't try to stop sound that's not there
        {
            UnityEngine.Debug.Log("Sound: " + name + " not found");
            return;
        }

        if (s.source != null)
            s.source.Stop();
        else
            UnityEngine.Debug.LogWarning("Sound: " + name + " has no AudioSource attached!");
    }

    public void StopAllAudio()
    {
        foreach (Sound s in sounds)
        {
            s.source.Stop();
        }

        if (introLoopCoroutine != null)
        {
            StopCoroutine(introLoopCoroutine);
            introLoopCoroutine = null;
        }
    }

    public void PauseAllAudio()
    {
        foreach (Sound s in sounds)
        {
            if (s.source != null && s.source.isPlaying)
            {
                s.source.Pause();
            }
        }

        if (introLoopCoroutine != null)
        {
            StopCoroutine(introLoopCoroutine);
            introLoopCoroutine = null;
        }
    }

    public void UnpauseAllAudio()
    {
        foreach (Sound s in sounds)
        {
            if (s.source != null)
            {
                s.source.UnPause();
            }
        }
    }

    public string CurrentlyPlaying()
    {
        foreach (Sound s in sounds)
        {
            if (s.source != null && s.source.isPlaying && s.isMusic)
            {
                return s.name; // return the first music that’s playing
            }
        }

        return null;
    }

    public IEnumerator FadeIn(string name, float fadeTime)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        s.source.volume = 0f;  // start silent
        s.source.Play();

        while (s.source.volume < s.volume)
        {
            s.source.volume += Time.deltaTime / fadeTime;
            yield return null;
        }

        s.source.volume = s.volume; // ensure final value is max
    }

    public IEnumerator FadeOut(string name, float fadeTime)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        float startVolume = s.source.volume;

        while (s.source.volume > 0)
        {
            s.source.volume -= startVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        s.source.Stop();
        s.source.volume = startVolume;
    }

    // To play a sound, add this line
    // AudioManager.instance.Play("SoundName");
}
