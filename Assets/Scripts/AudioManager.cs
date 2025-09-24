using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public struct Sound
{
    public string name;
    public AudioClip clip;
    private AudioSource source;

    public void Init(GameObject host)
    {
        source = host.AddComponent<AudioSource>();
        source.clip = clip;
        source.playOnAwake = false;
    }

    public void SetStartTime(float t)
    {
        if (source != null)
        {
            source.time = t;
        }
    }

    public void setVolume(float volume)
    {
        if (source != null)
        {
            source.volume = Mathf.Clamp(volume, 0f, 1f);
        }
    }

    public bool isPlaying()
    {
        if (source != null)
        {
            return source.isPlaying;
        }
        return false;
    }

    public void Play()
    {
        if (source != null)
        {
            Debug.Log("playing " + name);
            source.Play();
        }
    }

    public void Pause()
    {
        if (source != null)
        {
            source.Pause();
        }
    }

    public void UnPause()
    {
        if (source != null)
        {
            source.UnPause();
        }
    }

    public void Stop()
    {
        if (source != null)
        {
            source.Stop();
        }
    }

    public string getName()
    {
        return name;
    }
}

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    private Dictionary<string, int> soundIndices = new Dictionary<string, int>();

    void Start()
    {
        for (int i = 0; i < sounds.Count(); i++)
        {
            sounds[i].Init(gameObject);
            soundIndices.Add(sounds[i].name, i);
        }
    }

    private void Update()
    {

    }

    private Sound getSound(string name)
    {
        return sounds[soundIndices[name]];
    }

    public void setStartTime(string name, float t)
    {
        getSound(name).SetStartTime(t);
    }

    public void setVolume(string name, float volume)
    {
        getSound(name).setVolume(volume);
    }

    public bool isPlaying(string name)
    {
        return getSound(name).isPlaying();
    }

    public void playSound(string name)
    {
        getSound(name).Play();
    }

    public void pauseSound(string name)
    {
        getSound(name).Pause();
    }

    public void unPauseSound(string name)
    {
        getSound(name).UnPause();
    }

    public void stopSound(string name)
    {
        getSound(name).Stop();
    }

}
