using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioRecorder : MonoBehaviour
{
    public AudioSource audioSource;
    public int duration = 8;

    public AudioMixer testMixer;

    void Start()
    {
        Application.targetFrameRate = 60;
        
        audioSource.clip = Microphone.Start(string.Empty, audioSource.loop, duration, AudioSettings.outputSampleRate);

        audioSource.Play();
    }

}
