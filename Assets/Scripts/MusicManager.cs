using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    public AudioSource audioSource;
    public AudioSource mainMusicSource;
    public AudioSource fishingMusicSource;

    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }
    public void PlayMainMusic()
    {
        if (!mainMusicSource.isPlaying)
        {
            fishingMusicSource.Stop();
            mainMusicSource.Play();
        }
    }

    public void PlayFishingMusic()
    {
        if (!fishingMusicSource.isPlaying)
        {
            mainMusicSource.Stop();
            fishingMusicSource.Play();
        }
    }
}
