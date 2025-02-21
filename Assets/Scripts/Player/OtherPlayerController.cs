using System.Net;
using UnityEngine;
using UnityEngine.Audio;
using UnityOpus;

public class OtherPlayerController : MonoBehaviour
{
    private AudioSource audioSource;

    private int sampleRate = 48000;         // Recommended sample rate for Opus
    private int channels = 1;               // Mono is typical for voice chat
    private int frameSize;
    private Decoder decoder;

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        decoder = new Decoder(SamplingFrequency.Frequency_48000, NumChannels.Mono);
    }

    public void PlayAudio(byte[] data)
    {
        // Allocate a buffer for the decoded PCM data (float samples)
        float[] decodedPcm = new float[frameSize * channels];
        // Decode the data into the buffer. The Decoder.Decode method returns the total number of samples (across channels)
        int decodedSamples = decoder.Decode(data, data.Length, decodedPcm);
        if (decodedSamples <= 0)
        {
            Debug.LogWarning("Decoding failed or produced no samples.");
            return;
        }

        // Create an AudioClip from the decoded PCM data.
        AudioClip clip = AudioClip.Create("ReceivedVoice", decodedSamples, channels, sampleRate, false);
        clip.SetData(decodedPcm, 0);
        audioSource.PlayOneShot(clip);
    }
}
