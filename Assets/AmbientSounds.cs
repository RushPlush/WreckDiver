using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class AmbientSounds : MonoBehaviour
{
    [FormerlySerializedAs("ambientSounds")]
    [SerializeField] private List<AudioClip> ambientBackgroundSounds = new();
    [SerializeField] private List<AudioClip> ambientOverlaySounds = new();
    [SerializeField] private List<float> ambientSoundsVolume = new();
    [SerializeField] private List<float> ambientSoundsCooldown = new();
                     private List<float> ambientSoundsCooldownTimer = new();
    [SerializeField] private Vector2 ambientSoundsPitch = new(0.8f, 1.2f);
    [SerializeField] private Vector2 ambientMinTimeBetweenSounds = new(5f, 10f);
    [SerializeField] private static int maxSoundsToKeepTrackOf = 3;
                     private List<int> ambientSoundsToKeepTrackOf = new();
    private float timerBetweenSounds = 0f;
    [SerializeField] private AudioSource ambientOverlayAudioSource;
    [SerializeField] private AudioSource ambientBackgroundAudioSource;
    
    private void Awake()
    {
        //audioSource = gameObject.GetComponent<AudioSource>();
    }
    private void Start()
    {
        ambientSoundsCooldownTimer = new List<float>(new float[ambientOverlaySounds.Count]);
        if (ambientOverlaySounds.Count == 0)
        {
            Debug.LogWarning("No ambient sounds set in the inspector");
            return;
        }
        PlayRandomAmbientOverlaySound();
    }
    void PlayRandomAmbientOverlaySound()
    {
        //play a random ambient sound
        int randomIndex = UnityEngine.Random.Range(0, ambientOverlaySounds.Count);
        //audioSource.clip = ambientSounds[randomIndex];
        PlayOverlayAmbientSound(randomIndex);
    }
    private void PlayOverlayAmbientSound(int soundIndex)
    {
        if(ambientSoundsToKeepTrackOf.Count >= maxSoundsToKeepTrackOf)
        {
            ambientSoundsToKeepTrackOf.RemoveAt(0);
        }
        ambientSoundsToKeepTrackOf.Add(soundIndex);
        
        ambientOverlayAudioSource.volume = ambientSoundsVolume[soundIndex];
        ambientOverlayAudioSource.pitch = UnityEngine.Random.Range(ambientSoundsPitch.x, ambientSoundsPitch.y);
        ambientOverlayAudioSource.PlayOneShot(ambientOverlaySounds[soundIndex]);
        ambientSoundsCooldownTimer[soundIndex] = ambientSoundsCooldown[soundIndex] + ambientOverlaySounds[soundIndex].length;
        timerBetweenSounds = UnityEngine.Random.Range(ambientMinTimeBetweenSounds.x, ambientMinTimeBetweenSounds.y);
    }
    private void PlayBackgroundSound()
    {
        if (ambientBackgroundSounds.Count == 0) return;
        int randomIndex = UnityEngine.Random.Range(0, ambientBackgroundSounds.Count);
        ambientBackgroundAudioSource.clip = ambientBackgroundSounds[randomIndex];
        ambientBackgroundAudioSource.pitch = UnityEngine.Random.Range(ambientSoundsPitch.x, ambientSoundsPitch.y);
        ambientBackgroundAudioSource.Play();
    }
    private void Update()
    {
        timerBetweenSounds -= Time.deltaTime;
        for (int i = UnityEngine.Random.Range(0,ambientSoundsCooldown.Count-1); i < ambientSoundsCooldownTimer.Count; i++)
        {
            if (ambientSoundsCooldownTimer[i] > 0)
            {
                ambientSoundsCooldownTimer[i] -= Time.deltaTime;
            }
            else
            {
                if (timerBetweenSounds > 0) continue;
                if(ambientSoundsToKeepTrackOf.Contains(i)) continue;
                PlayOverlayAmbientSound(i);
            }
        }
        if (ambientBackgroundAudioSource.isPlaying) return;
        PlayBackgroundSound();
        
    }
}
