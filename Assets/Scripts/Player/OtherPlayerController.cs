using System.Net;
using UnityEngine;
using UnityEngine.Audio;

public class OtherPlayerController : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    public void PlayAudio(byte[] data)
    {
        // Implement audio decoding based on your format
        // Example using WAV format:
        AudioClip clip = WavUtility.ToAudioClip(data);
        audioSource.PlayOneShot(clip);
    }
}
