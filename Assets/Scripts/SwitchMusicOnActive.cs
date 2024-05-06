using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchMusicOnActive : MonoBehaviour
{
    public AudioClip[] playlist;

    public string sceneName;

    public string playlistName;

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.activeSceneChanged += ChangedActiveScene;
        ChangePlaylist();

        // Set the playlist name if it is not set
        if (playlistName == null || playlistName == "")
        {
            playlistName = "Playlist_" + sceneName + "_" + gameObject.name;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ChangedActiveScene(Scene current, Scene next)
    {
        Debug.Log("Changed active scene to " + next.name);

        // Check if we are changing to _this_ scene
        if (next.name != sceneName)
        {
            return;
        }

        Debug.Log("Switching music to " + sceneName);

        // Change the playlist
        ChangePlaylist();
    }

    void ChangePlaylist()
    {
        // Get the music manager
        MusicManager musicManager =
            GameObject.Find("MusicSource").GetComponent<MusicManager>();

        // Check if the music manager is found
        if (musicManager == null)
        {
            Debug.LogError("MusicManager not found");
            return;
        }

        // Change the playlist
        musicManager.ChangePlaylist(
            playlist, MusicChangeMode.OnMeasure, playlistName);
    }
}
