using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Sound
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

    public void setIsLooping(bool b)
    {
        if (source != null)
        {
            source.loop = b;
        }
    }

    public string getName()
    {
        return name;
    }

    public float getLength()
    {
        return source.clip.length;
    }

    //3D sound settings

    public void setSpatialBlend(float t)
    {
        if (source != null) {
            source.spatialBlend = Mathf.Clamp01(t);
        }
    }

    public void setDopplerLevel(float t)
    {
        if (source != null)
        {
            source.dopplerLevel = Mathf.Clamp(0, t, 5f);
        }
    }

    public void setVolumeRolloffMode(string s = "")
    {
        if (source != null)
        {
            if (s == "Linear")
            {
                source.rolloffMode = AudioRolloffMode.Linear;
            }
            else
            {
                source.rolloffMode = AudioRolloffMode.Logarithmic;
            }
        }
    }

    public void setMinDistance(float t)
    {
        if (source != null)
        {
            if (t >= source.maxDistance)
            {
                return;
            }
            source.minDistance = Mathf.Max(0, t);
        }
    }

    public void setMaxDistance(float t)
    {
        if (source != null)
        {
            if (t <= source.minDistance)
            {
                return;
            }
            source.maxDistance = Mathf.Max(0, t);
        }
    }
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public Sound[] sounds;
    private Dictionary<string, int> soundIndices = new Dictionary<string, int>();

    void Awake()
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
        if (!soundIndices.ContainsKey(name))
        {
            return null;
        }
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

    public void setIsLooping(string name, bool b)
    {
        getSound(name).setIsLooping(b);
    }

    public float getLength(string name)
    {
        return getSound(name).getLength();
    }

    //3D sound settings
    public void setSpatialBlend(string name, float t)
    {
        getSound(name).setSpatialBlend(t);
    }

    public void setDopplerLevel(string name, float t)
    {
        getSound(name).setDopplerLevel(t);
    }

    public void setVolumeRolloffMode(string name, string s)
    {
        getSound(name).setVolumeRolloffMode(s);
    }

    public void setMinDistance(string name, float t)
    {
        getSound(name).setMinDistance(t);
    }

    public void setMaxDistance(string name, float t)
    {
        getSound(name).setMaxDistance(t);
    }

    public void setAll3D(float maxDistance = 50f)
    {
        for (int i = 0; i < sounds.Count(); i++)
        {
            setSpatialBlend(sounds[i].name, 1);
            setDopplerLevel(sounds[i].name, 0);
            setVolumeRolloffMode(sounds[i].name, "Linear");
            setMinDistance(sounds[i].name, 0);
            setMaxDistance(sounds[i].name, maxDistance);
        }
        
    }
}
