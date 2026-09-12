using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneManager : MonoBehaviour
{
    public static MicrophoneManager instance;

    public AudioPitchEstimator pitchEstimator;

    private AudioClip _recordedAudioClip;

    [SerializeField]
    private AudioSource _audioSource;

    private float _latencyInSeconds = 0.01f;
    private int _latencyInSamples;

    private int _previousSample = 0;

    private float _currentLoudness = 0;
    private float _currentPitch = 0;

    private string _device;

    private void Awake()
    {
        Application.targetFrameRate = 60;    
    
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        this._device = Microphone.devices[0];
        
        this._latencyInSamples = Mathf.FloorToInt(AudioSettings.outputSampleRate * this._latencyInSeconds);

        this._recordedAudioClip = Microphone.Start(Microphone.devices[0], true, 1, AudioSettings.outputSampleRate);

        this._audioSource.clip = this._recordedAudioClip;
        this._audioSource.Play();
    }

    private void Update()
    {            
        int sampleDelta = this.GetDistanceFromCurrentSample(AudioSettings.outputSampleRate, this._previousSample, Microphone.GetPosition(this._device));

        if (sampleDelta > this._latencyInSamples)
        {
            float[] audioClipData = new float[sampleDelta];
            this._recordedAudioClip.GetData(audioClipData, this._previousSample);

            this._currentLoudness = this.GetPeakLoudness(audioClipData);
            
            this._previousSample = Microphone.GetPosition(Microphone.devices[0]);

            this._currentPitch = this.pitchEstimator.Estimate(this._audioSource);            

            //Debug.LogError("Current Loudness: " + this._currentLoudness);
        }
    }

    private int GetDistanceFromCurrentSample(int totalNumberOfSamples, int windowStart, int windowEnd)
    {
        if (windowEnd >= windowStart)
        {
            return windowEnd - windowStart;
        }

        return (windowEnd + totalNumberOfSamples) - windowStart;        
    }

    //Calculate Root Mean Square
    private float GetLoudness(float[] audioClipData)
    {
        float squareSum = 0;

        for (int i = 0; i < audioClipData.Length; i++)
        {
            squareSum = (audioClipData[i] * audioClipData[i]);
        }

        float averageOfSquares = squareSum / audioClipData.Length;

        return Mathf.Sqrt(averageOfSquares);
    }

    private float GetPeakLoudness(float[] audioClipData)
    {
        float peakValue = 0.0f;

        for (int i = 0; i < audioClipData.Length; i++)
        {
            float clipValue = Mathf.Abs(audioClipData[i]);
            if (clipValue > peakValue)
            { 
                peakValue = clipValue;
            }
        }

        return peakValue;
    }

    private float GetUnsignedPeakLoudness(float[] audioClipData)
    {
        float peakValue = 0.0f;

        for (int i = 0; i < audioClipData.Length; i++)
        {
            float clipValue = Mathf.Abs(audioClipData[i]);
            if (clipValue > peakValue)
            {
                peakValue = audioClipData[i];
            }
        }

        return peakValue;
    }

    public float GetRawLoudness()
    {
        return this._currentLoudness;
    }

    public float GetNormalizedPitch()
    {
        if (float.IsNaN(this._currentPitch))
        {
            return float.NaN;
        }
    
        float numerator = this._currentPitch - this.pitchEstimator.frequencyMin;
        float demoninator = this.pitchEstimator.frequencyMax - this.pitchEstimator.frequencyMin;        

        return (numerator / demoninator);
    }
}
