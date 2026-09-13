using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneManager : MonoBehaviour
{
    private const int WINDOW_SIZE = 3;    

    public static MicrophoneManager instance;

    public AudioPitchEstimator pitchEstimator;

    public float minClampedPitch = 100;
    public float maxClampedPitch = 400;

    public float anomalousPitchDiffThreshold = 0.7f;

    public float noiseGate = 0.001f;

    private AudioClip _recordedAudioClip;

    [SerializeField]
    private AudioSource _audioSource;

    private float _latencyInSeconds = 0.01f;
    private int _latencyInSamples;

    private int _previousSample = 0;

    private float _currentLoudness = 0;
    private float _currentPitch = 0;

    private string _device;

    private Queue<float> _previousPitches;

    private float _previousAverage = 0;

    public float volumeMultiplier = 3.0f;

    private float _minVolume = 0.0f;
    private float _maxVolume = 0.2f;

    private void Awake()
    {
        Application.targetFrameRate = 144;    
    
        if (instance == null)
        {
            instance = this;
        }

        this._previousPitches = new Queue<float>();
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
            this.UpdateCurrentLoudness(sampleDelta);

            //Debug.LogError("Current Loudness: " + this._currentLoudness);

            if (this._currentLoudness > this.noiseGate)
            {
                this.UpdateCurrentPitch();
            }          
            else
            {
                Debug.LogError("NOT LOUD ENOUGH! " + this._currentLoudness);
                this.ResetPitchAverage();
            }
        }
    }

    private void UpdateCurrentLoudness(int sampleDelta)
    {
        float[] audioClipData = new float[sampleDelta];
        this._recordedAudioClip.GetData(audioClipData, this._previousSample);

        this._currentLoudness = this.GetPeakLoudness(audioClipData);

        this._previousSample = Microphone.GetPosition(Microphone.devices[0]);
    }

    private void UpdateCurrentPitch()
    {
        float latestRawPitchEstimate = this.pitchEstimator.Estimate(this._audioSource);

        if (float.IsNaN(latestRawPitchEstimate) == true)
        {
            return;
        }

        //Debug.LogError("New Pitch: " + latestRawPitchEstimate + "\nNormalized: " + this.NormalizePitchValue(latestRawPitchEstimate) + " Average: " + this._previousAverage);

        float latestNormalizedPitchEstimate = this.NormalizePitchValue(latestRawPitchEstimate);

        if (this.IsPitchAnomalous(latestNormalizedPitchEstimate) == false)
        {
            this.UpdatePreviousPitchAverage(latestNormalizedPitchEstimate);
            this._currentPitch = latestRawPitchEstimate;
        }
        else
        {
            //Debug.LogError("Anomalous Pitch. SKIPPING!");
        }
    }

    private bool IsPitchAnomalous(float testPitch)
    {
        //Debug.LogError("testPitch: " + testPitch + " Average: " + this._previousAverage + "\nDiff: " + Mathf.Abs(testPitch - this._previousAverage));

        if (this._previousPitches.Count == 0)
        {
            return false;
        }

        return (Mathf.Abs(testPitch - this._previousAverage) > this.anomalousPitchDiffThreshold);
    }

    private void UpdatePreviousPitchAverage(float newPitch)
    {
        float intermediateTotal = this._previousAverage * this._previousPitches.Count;
        if (this._previousPitches.Count == WINDOW_SIZE)
        {
            float poppedValue = this._previousPitches.Dequeue();
            intermediateTotal -= poppedValue;
        }

        this._previousPitches.Enqueue(newPitch);

        intermediateTotal += newPitch;

        this._previousAverage = intermediateTotal / (float)this._previousPitches.Count;
    }

    private void ResetPitchAverage()
    {
        this._previousAverage = 0.0f;
        this._previousPitches.Clear();
    }

    private int GetDistanceFromCurrentSample(int totalNumberOfSamples, int windowStart, int windowEnd)
    {
        if (windowEnd >= windowStart)
        {
            return windowEnd - windowStart;
        }

        return (windowEnd + totalNumberOfSamples) - windowStart;        
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

    public float GetRawLoudness()
    {
        return this._currentLoudness;
    }

    public float GetScaledLoudness()
    {
        return this._currentLoudness * this.volumeMultiplier;
    }

    public float GetNormalizedLoudness()
    {

        float numerator = this.GetScaledLoudness() - this._minVolume;
        float denominator = this._maxVolume - this._minVolume;

        float resultant = Mathf.Clamp((numerator / denominator), 0.0f, 1.0f);

        return resultant;
    }

    public float GetRawPitch()
    {
        return this._currentPitch;
    }

    public float GetCurrentNormalizedPitch()
    {
        if (float.IsNaN(this._currentPitch))
        {
            return float.NaN;
        }

        float numerator = this._currentPitch - this.minClampedPitch;
        float denominator = this.maxClampedPitch - this.minClampedPitch;

        float resultant = Mathf.Clamp((numerator / denominator), 0.0f, 1.0f);

        return resultant;
    }

    public float NormalizePitchValue(float pitch)
    {
        if (float.IsNaN(pitch))
        {
            return float.NaN;
        }

        float numerator = pitch - this.minClampedPitch;
        float demoninator = this.maxClampedPitch - this.minClampedPitch;

        float resultant = Mathf.Clamp((numerator / demoninator), 0.0f, 1.0f);

        return resultant;
    }
}
