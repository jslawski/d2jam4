using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class MicrophoneManager : MonoBehaviour
{
    private const int WINDOW_SIZE = 3;    

    public static MicrophoneManager instance;

    public AudioPitchEstimator pitchEstimator;

    [HideInInspector]
    public float minClampedPitch = 100;
    [HideInInspector]
    public float maxClampedPitch = 400;

    private float anomalousPitchDiffThreshold = 150f;

    [HideInInspector]
    public float noiseGate = 0.01f;

    //private AudioClip _recordedAudioClip;

    [SerializeField]
    private AudioMixerGroup _audioMixerGroup;

    //[SerializeField]
    private AudioSource _audioSource;

    private float _latencyInSeconds = 0.01f;
    private int _latencyInSamples;

    private int _previousSample = 0;

    private float _currentLoudness = 0;
    private float _currentPitch = 0;

    private Queue<float> _previousPitches;

    private float _previousAverage = 0;

    private float _minVolume = 0.0f;
    private float _maxVolume = 0.02f;

    private bool _micActive = true;

    private int _micSampleRate = 48000;

    private void Awake()
    {

        int minFreq = 0;
        int maxFreq = 0;

        for (int i = 0; i < Microphone.devices.Length; i++)
        {        
            Microphone.GetDeviceCaps(Microphone.devices[i], out minFreq, out maxFreq);

            Debug.LogError(Microphone.devices[i] + "\nMin: " + minFreq + " Max: " + maxFreq);
        }
        
          
        DontDestroyOnLoad(this);

        instance = this;

        GameOptions.device = Microphone.devices[4];
        Microphone.GetDeviceCaps(Microphone.devices[4], out minFreq, out maxFreq);
        AudioSettings.outputSampleRate = maxFreq;

        this._previousPitches = new Queue<float>();

        this._audioSource = this.gameObject.AddComponent<AudioSource>();
        this._audioSource.playOnAwake = false;
        this._audioSource.loop = true;
        this._audioSource.outputAudioMixerGroup = this._audioMixerGroup;

        this.pitchEstimator = GetComponent<AudioPitchEstimator>();

        //this.pitchEstimator = this.gameObject.AddComponent<AudioPitchEstimator>();

        //this._audioSource.clip = this._recordedAudioClip;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Application.targetFrameRate = 144;        

        this._latencyInSamples = Mathf.FloorToInt(AudioSettings.outputSampleRate * this._latencyInSeconds);

        this._audioSource.clip = Microphone.Start(GameOptions.device, true, 1, AudioSettings.outputSampleRate);
        //this._audioSource.clip = this._recordedAudioClip;
        //this._audioSource.clip = this._recordedAudioClip;
        this._audioSource.Play();
    }

    private void Update()
    {
        if (this._micActive == false)
        {
            return;
        }
        
        int sampleDelta = this.GetDistanceFromCurrentSample(AudioSettings.outputSampleRate, this._previousSample, Microphone.GetPosition(GameOptions.device));

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
                //Debug.LogError("NOT LOUD ENOUGH! " + this._currentLoudness);
                this.ResetPitchAverage();
            }
        }
    }

    private void UpdateCurrentLoudness(int sampleDelta)
    {
        float[] audioClipData = new float[sampleDelta];
        this._audioSource.clip.GetData(audioClipData, this._previousSample);

        this._currentLoudness = this.GetPeakLoudness(audioClipData);

        //Debug.LogError("RAW LOUDNESS: " + this._currentLoudness);

        this._previousSample = Microphone.GetPosition(GameOptions.device);
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

        if (this.IsPitchAnomalous(latestRawPitchEstimate) == false)
        {
            this.UpdatePreviousPitchAverage(latestRawPitchEstimate);
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

    public bool IsPitchAnomalous(List<float> testWindow, float testPitch)
    {
        float total = 0.0f;

        for (int i = 0; i < testWindow.Count; i++)
        {
            total += testWindow[i];
        }

        float average = total / testWindow.Count;

        return (Mathf.Abs(testPitch - average) > this.anomalousPitchDiffThreshold);
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
        return this._currentLoudness * GameOptions.volumeMultiplier;
    }

    public float GetNormalizedLoudness()
    {
        if (this.GetScaledLoudness() < this.noiseGate)
        {
            return 0.0f;
        }


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

    public void ActivateMicInput()
    {
        this._micActive = true;
    }

    public void Refresh()
    {
        return;
    
        this._audioSource.Stop();    
    
        //this._recordedAudioClip = Microphone.Start(GameOptions.device, true, 1, AudioSettings.outputSampleRate);

        //this._audioSource.clip = this._recordedAudioClip;
        this._audioSource.Play();
    }

    public void StopMicInput()
    {
        this._micActive = false;
    }

    public void ClearAudioData()
    {
        Microphone.End(GameOptions.device);
        this._audioSource.clip = null;
        //this._recordedAudioClip = null;        
        this._audioSource.Stop();

        Destroy(this.pitchEstimator);
    }
}
