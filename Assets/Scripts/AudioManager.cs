using System;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine;

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

    void Start()
    {
        Play("Theme");
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

    // 🔑 Hard stop everything including coroutines
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

        s.source.Stop();
    }

    // To play a sound, add this line
    // FindObjectOfType<AudioManager>().Play("SoundName");
}
