using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MusicChangeMode
{
    OnBeat,
    OnMeasure,
    OnSection,
    OnSongEnd
}

public class MusicManager : MonoBehaviour
{
    // BPM of the music for sync
    public float bpm = 195.0f;

    // Current playlist
    public AudioClip[] playlist;
    int currentSong = 0;

    // Transition
    public AudioClip transition;
    public int transitionLengthBeats = 16;

    // Audio source
    AudioSource[] audioSources = new AudioSource[2];
    int currentAudioSource = 0;

    AudioSource transitionSource = new AudioSource();

    // Next event
    double nextEventTime;

    // Are we playing
    bool running = false;

    // Current playlist name
    public string currentPlaylistName;

    // Start is called before the first frame update
    void Start()
    {
        // Get the audio source
        audioSources[0] = gameObject.AddComponent<AudioSource>();
        audioSources[1] = gameObject.AddComponent<AudioSource>();

        transitionSource = gameObject.AddComponent<AudioSource>();

        // Set the audio source to loop
        audioSources[0].loop = false;
        audioSources[1].loop = false;

        // Set the transition source to loop
        transitionSource.loop = false;

        // Set the transition source clip
        transitionSource.clip = transition;

        // Set the transition source volume
        transitionSource.volume = 0.7f;

        // Set the next event time
        nextEventTime = AudioSettings.dspTime + 1.0f;

        // We're running
        running = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!running)
        {
            return;
        }

        // Queue up the next song if the current song is < 1 measure from the end
        // Only do this once per song
        double currentTime = AudioSettings.dspTime;

        if (currentTime + 60.0f / bpm * 4 > nextEventTime)
        {
            // Switch audio sources
            currentAudioSource = 1 - currentAudioSource;

            // Load the next song
            audioSources[currentAudioSource].clip = playlist[currentSong];
            audioSources[currentAudioSource].PlayScheduled(nextEventTime);
            audioSources[1 - currentAudioSource].SetScheduledEndTime(nextEventTime);

            // Increment the song
            currentSong = (currentSong + 1) % playlist.Length;

            // Schedule the next song
            nextEventTime += playlist[currentSong].length;
        }
    }

    // Replace playlist and start playing the first song
    public void ChangePlaylist(
        AudioClip[] newPlaylist,
        MusicChangeMode mode = MusicChangeMode.OnBeat,
        string playlistName = ""
    )
    {
        Debug.Log("Changing playlist");
        Debug.Log("Mode: " + mode);
        Debug.Log("Playlist name: " + playlistName);
        Debug.Log("Current playlist name: " + currentPlaylistName);

        // If the same playlist is already playing, do nothing
        if (playlistName == currentPlaylistName)
        {
            return;
        }
        else
        {
            currentPlaylistName = playlistName;
        }

        // Get the current time
        double currentTime = AudioSettings.dspTime;
        double currentTimeInSong = audioSources[currentAudioSource].time;

        playlist = newPlaylist;
        currentSong = 0;

        // Load the first song
        if (mode == MusicChangeMode.OnBeat)
        {
            // How far through the current measure are we in the current song?
            double measureLength = (60.0f / bpm) * 2;
            double timeToNextMeasure = measureLength -
                (currentTimeInSong % measureLength);

            // Schedule the next song to start at the next measure
            nextEventTime = currentTime + timeToNextMeasure;
        }
        else if (mode == MusicChangeMode.OnMeasure)
        {
            // How far through the current measure are we in the current song?
            double measureLength = (60.0f / bpm) * 8;
            double timeToNextMeasure = measureLength -
                (currentTimeInSong % measureLength);

            // Schedule the next song to start at the next measure
            nextEventTime = currentTime + timeToNextMeasure + measureLength;

            // Play the second half of the transition
            if (nextEventTime - (transition.length / 2) < currentTime)
            {
                transitionSource.time = (float)(currentTime - (nextEventTime - transition.length));
            }
            transitionSource.time = transition.length / 2;
            transitionSource.PlayScheduled(nextEventTime - (transition.length / 2));
            transitionSource.SetScheduledEndTime(nextEventTime);
        }
        else if (mode == MusicChangeMode.OnSection)
        {
            // How far through the current measure are we in the current song?
            double measureLength = (60.0f / bpm) * 32;
            double timeToNextMeasure = measureLength -
                (currentTimeInSong % measureLength);

            // Schedule the next song to start at the next measure
            nextEventTime = currentTime + timeToNextMeasure;

            // Play the transition
            if (nextEventTime - transition.length < currentTime)
            {
                transitionSource.time = (float)(currentTime - (nextEventTime - transition.length));
            }
            transitionSource.PlayScheduled(nextEventTime - transition.length);
            transitionSource.SetScheduledEndTime(nextEventTime);
        }
        else if (mode == MusicChangeMode.OnSongEnd)
        {
            // Schedule the next song to start at the end of the current song
            nextEventTime = currentTime +
                audioSources[currentAudioSource].clip.length;

            // Play the transition
            if (nextEventTime - transition.length < currentTime)
            {
                transitionSource.time = (float)(currentTime - (nextEventTime - transition.length));
            }
            transitionSource.PlayScheduled(nextEventTime - transition.length);
            transitionSource.SetScheduledEndTime(nextEventTime);
        }
    }
}
