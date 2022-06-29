using UnityEngine.Audio;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public bool isMuted;

    public static AudioManager instance;


    private List<float> soundsVolumes = new List<float> ();
    
    // Start is called before the first frame update
    void Awake()
    {

        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Globals.audioManager = this;

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;

            s.source.loop = s.loop;

            soundsVolumes.Add (s.volume);
        }
    }

    private void Start()
    {
        FadeIn("menuTheme", 2);
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Play:: Sound "+name+" not found");
            return;
        }
        if (!isMuted)
            s.source.volume = s.volume;
        else
            s.source.volume = 0;
        s.source.Play();
    }
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Stop:: Sound " + name + " not found");
            return;
        }
        s.source.Stop();
    }
    public void FadeIn(string name, float timing)
    {
        StartCoroutine(FadeInC(name, timing));
    }
    public void FadeOut(string name, float timing)
    {
        StartCoroutine(FadeOutC(name, timing));
    }
    public IEnumerator FadeInC(string name, float timing)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("FadeOut:: Sound " + name + " not found");
            yield break;
        }
        s.source.volume = 0;
        Play(name);
        float speed = s.volume / timing;
        while (Mathf.Abs(s.source.volume - s.volume) > 0.05f)
        {
            s.source.volume += speed * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
    public IEnumerator FadeOutC(string name, float timing)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("FadeOut:: Sound " + name + " not found");
            yield break;
        }
        s.source.volume = s.volume;
        float speed = s.volume / timing;
        while(s.source.volume > 0.05f)
        {
            s.source.volume -= speed * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        Stop(name);
    }

    public void updateMuteState () {
        isMuted = Globals.mainSettings.muteAudio;
        if (isMuted) {
            foreach (Sound s in sounds) {
                s.volume = 0;
                s.source.volume = 0;
            }
        } else {
            for(int i = 0; i < sounds.Length; i++) {
                sounds[i].source.volume = soundsVolumes[i];
                sounds[i].volume = soundsVolumes[i];
            }
        }
    }
}
