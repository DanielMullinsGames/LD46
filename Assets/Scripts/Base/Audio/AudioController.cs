using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using Pixelplacement;

public class AudioController : MonoBehaviour
{
    public AudioSource BaseLoopSource
    {
        get
        {
            return loopSources[0];
        }
    }

    List<AudioClip> SFX = new List<AudioClip>();
    List<AudioClip> Loops = new List<AudioClip>();
    public static AudioController Instance { get; private set; }

    [SerializeField]
    private List<AudioSource> loopSources = default;

    private List<AudioSource> ActiveSFXSources
    {
        get
        {
            activeSFX.RemoveAll(x => x == null || ReferenceEquals(x, null));
            return activeSFX;
        }
    }
    private List<AudioSource> activeSFX = new List<AudioSource>();

    public bool Fading { get; set; }

    private Dictionary<string, float> limitedFrequencySounds = new Dictionary<string, float>();
    private Dictionary<string, int> lastPlayedSounds = new Dictionary<string, int>();

    private List<AudioMixer> loadedMixers = new List<AudioMixer>();
    private AudioMixerGroup currentSFXMixer = default;

    private const string SOUNDID_REPEAT_DELIMITER = "#";
    private const float DEFAULT_SPATIAL_BLEND = 0.75f;

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        transform.parent = null;
        DontDestroyOnLoad(gameObject);

        foreach (object o in Resources.LoadAll("Audio/SFX"))
        {
            SFX.Add((AudioClip)o);
        }
        foreach (object o in Resources.LoadAll("Audio/Loops"))
        {
            Loops.Add((AudioClip)o);
        }
    }

    #region SFX
    public AudioSource PlaySound2D(string soundId, MixerGroup mixerGroup = MixerGroup.None, float volume = 1f, float skipToTime = 0f, AudioParams.Pitch pitch = null,
        AudioParams.Repetition repetition = null, AudioParams.Randomization randomization = null, AudioParams.Distortion distortion = null, bool looping = false)
    {
        var source = PlaySound3D(soundId, Vector3.zero, mixerGroup, volume, skipToTime, pitch, repetition, randomization, distortion, looping);

        if (source != null)
        {
            source.spatialBlend = 0f;
        }

        return source;
    }

    public AudioSource PlaySound3D(string soundId, Vector3 position, MixerGroup mixerGroup = MixerGroup.None, float volume = 1f, float skipToTime = 0f, AudioParams.Pitch pitch = null,
        AudioParams.Repetition repetition = null, AudioParams.Randomization randomization = null, AudioParams.Distortion distortion = null, bool looping = false)
    {
        if (repetition != null)
        {
            if (RepetitionIsTooFrequent(soundId, repetition.minRepetitionFrequency, repetition.entryId))
            {
                return null;
            }
        }

        string randomVariationId = soundId;
        if (randomization != null)
        {
            randomVariationId = GetRandomVariationOfSound(soundId, randomization.noRepeating);
        }

        var source = CreateAudioSourceForSound(randomVariationId, position, looping, mixerGroup);
        if (source != null)
        {
            source.volume = volume;
            source.time = source.clip.length * skipToTime;

            if (pitch != null)
            {
                source.pitch = pitch.pitch;
            }

            if (distortion != null)
            {
                if (distortion.muffled)
                {
                    MuffleSource(source);
                }
            }
        }

        activeSFX.Add(source);
        return source;
    }

    public void SetAllSoundsPaused(bool paused)
    {
        ActiveSFXSources.ForEach(x =>
        {
            if (paused)
            {
                x.Pause();
            }
            else
            {
                x.UnPause();
            }
        });
    }

    public void FadeSourceVolume(AudioSource source, float volume, float duration)
    {
        Tween.Volume(source, volume, duration, 0f);
    }

    private AudioSource CreateAudioSourceForSound(string soundId, Vector3 position, bool looping, MixerGroup mixerGroup = MixerGroup.None)
    {
        if (!string.IsNullOrEmpty(soundId))
        {
            AudioClip sound = SFX.Find(x => x.name.ToLower() == soundId.ToLower());

            if (sound != null)
            {
                return InstantiateAudioObject(sound, position, looping);
            }
        }

        return null;
    }

    private AudioSource InstantiateAudioObject(AudioClip clip, Vector3 pos, bool looping)
    {
        GameObject tempGO = new GameObject("Audio_" + clip.name);
        tempGO.transform.position = pos;

        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = clip;
        aSource.outputAudioMixerGroup = currentSFXMixer;
        aSource.spatialBlend = DEFAULT_SPATIAL_BLEND;

        aSource.Play();
        if (looping)
        {
            aSource.loop = true;
        }
        else
        {
            Destroy(tempGO, clip.length);
        }
        return aSource;
    }

    private bool RepetitionIsTooFrequent(string soundId, float frequencyMin, string entrySuffix = "")
    {
        float time = Time.unscaledTime;
        string soundKey = soundId + entrySuffix;

        if (limitedFrequencySounds.ContainsKey(soundKey))
        {
            if (time - frequencyMin > limitedFrequencySounds[soundKey])
            {
                limitedFrequencySounds[soundKey] = time;
                return false;
            }
        }
        else
        {
            limitedFrequencySounds.Add(soundKey, time);
            return false;
        }

        return true;
    }

    private string GetRandomVariationOfSound(string soundPrefix, bool noRepeating)
    {
        string soundId = "";

        if (!string.IsNullOrEmpty(soundPrefix))
        {
            List<AudioClip> variations = SFX.FindAll(x => x != null && x.name.ToLower().StartsWith(soundPrefix.ToLower() + SOUNDID_REPEAT_DELIMITER));

            if (variations.Count > 0)
            {
                int index = Random.Range(0, variations.Count) + 1;
                if (noRepeating)
                {
                    if (!lastPlayedSounds.ContainsKey(soundPrefix))
                    {
                        lastPlayedSounds.Add(soundPrefix, index);
                    }
                    else
                    {
                        int breakOutCounter = 0;
                        const int BREAK_OUT_THRESHOLD = 100;
                        while (lastPlayedSounds[soundPrefix] == index && breakOutCounter < BREAK_OUT_THRESHOLD)
                        {
                            index = Random.Range(0, variations.Count) + 1;
                            breakOutCounter++;
                        }

                        if (breakOutCounter >= BREAK_OUT_THRESHOLD - 1)
                        {
                            Debug.Log("Broke out of infinite loop! AudioController.PlayRandomSound.");
                        }

                        lastPlayedSounds[soundPrefix] = index;
                    }
                }

                soundId = soundPrefix + SOUNDID_REPEAT_DELIMITER + index;
            }
            else
            {
                soundId = soundPrefix;
            }
        }

        return soundId;
    }

    private void MuffleSource(AudioSource source, float cutoff = 300f)
    {
        var filter =source.gameObject.AddComponent<AudioLowPassFilter>();
        filter.cutoffFrequency = cutoff;
    }
    #endregion

    #region Loops
    public void SetLoopPaused(bool paused) 
    {
        foreach (AudioSource loopSource in loopSources)
        {
            if (paused)
            {
                loopSource.Pause();
            }
            else
            {
                loopSource.UnPause();
            }
        }
    }

    public void ResumeLoop(float fadeInSpeed = float.MaxValue) 
    {
        foreach (AudioSource loopSource in loopSources)
        {
            loopSource.UnPause();

            if (!loopSource.isPlaying)
            {
                loopSource.Play();
            }
        }
    }

    public void RestartLoop(int sourceIndex = 0)
    {
        loopSources[sourceIndex].Stop();
        loopSources[sourceIndex].time = 0f;
        loopSources[sourceIndex].volume = 1f;
        loopSources[sourceIndex].Play();
    }

    public void StopAllLoops() 
    {
        CancelFades();
        foreach (AudioSource loopSource in loopSources)
        {
            loopSource.Stop();
        }
    }

    public void StopLoop(int sourceIndex = 0)
    {
        loopSources[sourceIndex].Stop();
    }

    public void SetLoopAndPlay(string loopName, int sourceIndex = 0)
    {
        CancelFades();
        TrySetLoop(loopName, sourceIndex);
        RestartLoop(sourceIndex);
    }
	
	public void CrossFadeLoop(string loopName, float duration, float volume = 1f) 
    {
		for (int i = 0; i < Loops.Count; i ++) 
        {
			AudioClip clip = Loops[i];
			if (clip.name == loopName) 
            {
                CancelFades();
				StartCoroutine(CrossFade(loopName, volume, duration));
				return;
			}
		}
	}
	
	public void FadeOutLoop(float fadeDuration = float.MaxValue)
    {
        CancelFades();
		StartCoroutine(DoFadeToVolume(fadeDuration, 0f));
	}

	public void FadeInLoop(float fadeDuration = float.MaxValue, float toVolume = 1f)
    {
        CancelFades();
		StartCoroutine(DoFadeToVolume(fadeDuration, toVolume));
	}

    public void SetLoopCutoff(float cutoff)
    {
        var filter = GetComponent<AudioLowPassFilter>();
        if (filter == null)
        {
            filter = gameObject.AddComponent<AudioLowPassFilter>();
        }

        filter.cutoffFrequency = cutoff;
    }

    public void SetLoopVolumeImmediate(float volume, int sourceIndex = 0)
    {
        CancelFades();
        loopSources[sourceIndex].volume = volume;
    }

    public void SetLoopVolume(float volume, float duration, int sourceIndex = 0)
    {
        CancelFades();
        StartCoroutine(DoFadeToVolume(duration, volume, sourceIndex));
    }

    private void CancelFades()
    {
        StopAllCoroutines();
        foreach (AudioSource loopSource in loopSources)
        {
            Tween.Cancel(loopSource.GetInstanceID());
        }
        Fading = false;
    }

    private void TrySetLoop(string loopName, int sourceIndex = 0)
    {
        AudioClip loop = GetLoop(loopName);

        if (loop != null)
        {
            loopSources[sourceIndex].clip = loop;
        }
    }

    private AudioClip GetLoop(string loopName)
    {
        return Loops.Find(x => x.name == loopName);
    }

    private IEnumerator DoFadeToVolume(float duration, float volume, int sourceIndex = 0)
	{
        Fading = true;

        Tween.Volume(loopSources[sourceIndex], volume, duration, 0f, Tween.EaseInOut);
        yield return new WaitForSeconds(duration);

        Fading = false;
    }
	
    // TODO: make this ACTUALLY crossfade...
	private IEnumerator CrossFade (string newLoop, float volume, float duration, int sourceIndex = 0)
    {
        if (loopSources[0].clip != null && loopSources[0].isPlaying)
        {
            yield return DoFadeToVolume(duration * 0.5f, 0f);
        }

        TrySetLoop(newLoop);
        loopSources[0].Play();

        yield return DoFadeToVolume(duration * 0.5f, volume);
	}
    #endregion

    #region Mixing
    public void ClearLoopMixing()
    {
        foreach (AudioSource loopSource in loopSources)
        {
            loopSource.outputAudioMixerGroup = null;
        }
    }

    public void SetLoopMixing(string mixerId, string groupId, int sourceIndex = 0)
    {
        loopSources[sourceIndex].outputAudioMixerGroup = GetMixerGroup(mixerId, groupId);
    }


    public AudioMixerGroup GetMixerGroup(string mixerId, string groupId)
	{
		var loadedMixer = loadedMixers.Find (x => x.name == mixerId);
		if (loadedMixer == null)
		{
			loadedMixer = Resources.Load<AudioMixer>("Audio/Mixing/" + mixerId);
			loadedMixers.Add (loadedMixer);
		}

		if (loadedMixer != null)
		{
			var groups = loadedMixer.FindMatchingGroups (groupId);
			if (groups.Length > 0)
			{
				return groups [0];
			}
		}

		return null;
	}

    public IEnumerator FadeMixerParameter(string mixer, string channel, string parameter, float fadeTo, float fadeLength)
    {
        var mainBGM = GetMixerGroup(mixer, channel);
        float startVolume; mainBGM.audioMixer.GetFloat(parameter, out startVolume);
        float timer = 0f;

        while (timer < fadeLength)
        {
            timer += Time.deltaTime;
            mainBGM.audioMixer.SetFloat(parameter, Mathf.Lerp(startVolume, fadeTo, timer / fadeLength));
            yield return new WaitForEndOfFrame();
        }

        mainBGM.audioMixer.SetFloat(parameter, fadeTo);
    }
    #endregion
}
