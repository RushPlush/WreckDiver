using System;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        var scene = SceneManager.GetActiveScene();
        if (scene == SceneManager.GetSceneByBuildIndex(0))
        {
            var menuInstructions = FindFirstObjectByType<MenuInstructions>();
            if (!menuInstructions.PlayMenuAnimaticOnAwake)
            {
                player.SetActive(true);
                animaticCamera.SetActive(false);
                videoPlayer.Stop();
            }
        }
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
