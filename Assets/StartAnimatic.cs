using System;
using UnityEngine;
using UnityEngine.Video;

public class StartAnimatic : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject animaticCamera;
    private VideoPlayer videoPlayer;

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
    }

    private void Start()
    {
        //videoPlayer.Play();
    }

    private void Update()
    {
        if (!videoPlayer.isPlaying && Time.timeSinceLevelLoad > videoPlayer.clip.length)
        {
            player.SetActive(true);
            animaticCamera.SetActive(false);
        }
    }
}
