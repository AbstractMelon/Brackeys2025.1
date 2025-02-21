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
        Debug.Log(data);
        AudioClip clip = WavUtility.ToAudioClip(data);
        audioSource.PlayOneShot(clip);
    }
}
