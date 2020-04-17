using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioKeyframeEvents : MonoBehaviour
{
    [System.Serializable]
    public class KeyframeEvent
    {
        public string id;

        [Header("Audio Params")]
        public string audioId;
        public MixerGroup mixerGroup;
        public bool is2D;
        public float volume = 1f;

        public float skipToNormalizedTime = 0f;

        public bool randomPitch;
        public AudioParams.Pitch.Variation pitchVariation;

        public bool limitRepetition;
        public float minRepeatTime;

        public bool randomize;

        [Header("3D Sound Params")]
        public bool modified3DSoundParams;
        public float minDistance;
        public float maxDistance;

        public void Play(Transform transform)
        {
            AudioParams.Pitch pitch = null;
            if (randomPitch)
            {
                pitch = new AudioParams.Pitch(pitchVariation);
            }

            AudioParams.Repetition repetition = null;
            if (limitRepetition)
            {
                repetition = new AudioParams.Repetition(minRepeatTime);
            }

            AudioParams.Randomization randomization = null;
            if (randomize)
            {
                randomization = new AudioParams.Randomization();
            }

            if (is2D)
            {
                AudioController.Instance.PlaySound2D(audioId, mixerGroup, volume, skipToNormalizedTime, pitch, repetition, randomization);
            }
            else
            {
                var source = AudioController.Instance.PlaySound3D(audioId, transform.position, mixerGroup, volume, skipToNormalizedTime, pitch, repetition, randomization);

                if (modified3DSoundParams)
                {
                    source.Stop();
                    source.minDistance = minDistance;
                    source.maxDistance = maxDistance;
                    source.rolloffMode = AudioRolloffMode.Linear;
                    source.spatialBlend = 1f;
                    source.Play();
                }
            }
        }
    }

    public List<KeyframeEvent> events = new List<KeyframeEvent>();

    public void FireKeyframeEvent(string keyframeId)
    {
        var keyframeEvent = events.Find(x => x.id.ToLower() == keyframeId.ToLower());

        if (keyframeEvent != null)
        {
            keyframeEvent.Play(transform);
        }
    }
}
