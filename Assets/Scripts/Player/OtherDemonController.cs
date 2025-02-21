using UnityEngine;

public class OtherDemonController : MonoBehaviour
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
