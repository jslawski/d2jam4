using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class MicrophoneReader : MonoBehaviour
{
    public static MicrophoneReader instance;    

    private AudioClip _recordedAudioClip;

    private float _timeBetweenReadsInSeconds = 10f;

    private int _samplesBetweenReads = 0;

    private int _sampleWindow = 15;

    private string _device;

    private float _currentLoudness = 0;

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
        for (int i = 0; i < Microphone.devices.Length; i++)
        {
            Debug.LogError(Microphone.devices[i]);
        }

        this._device = Microphone.devices[0];

        this.StartRecording(this._device);

        this._samplesBetweenReads = Mathf.CeilToInt(AudioSettings.outputSampleRate / this._timeBetweenReadsInSeconds);        
    }

    public void StartRecording(string deviceName)
    {
        this._recordedAudioClip = Microphone.Start(deviceName, true, 1, AudioSettings.outputSampleRate);
        StartCoroutine(this.ReadInput());
    }

    public void StopRecording(string deviceName)
    {
        Microphone.End(deviceName);
    }

    public float GetCurrentLoudness(string deviceName)
    {
        if (Microphone.IsRecording(deviceName) == false)
        {
            Debug.LogError("ERROR: Microphone is not currently recording. Make sure to call MicrophoneReader.StartRecording() first.");
            return 0.0f;
        }

        int currentPosition = Microphone.GetPosition(this._device);
        int startPosition = currentPosition - this._sampleWindow;
        float[] sampleData = new float[this._sampleWindow];

        if (startPosition < 0)
        {
            return 0.0f;            
        }

        this._recordedAudioClip.GetData(sampleData, startPosition);

        return this.GetAverageLoudness(sampleData);
    }

    private float GetAverageLoudness(float[] sampleData)
    {
        float totalLoudness = 0.0f;

        for (int i = 0; i < this._sampleWindow; i++)
        {
            totalLoudness += Mathf.Abs(sampleData[i]);
        }
        
        return (totalLoudness / this._sampleWindow);
    }

    public float GetRawLoudness()
    {
        return this._currentLoudness;
    }


    private IEnumerator ReadInput()
    {

        while (true)
        {
            Debug.LogError("Looping");
        
        float[] fullAudioClipData = new float[AudioSettings.outputSampleRate];

            int currentMicPosition = Microphone.GetPosition(this._device);

            this._recordedAudioClip.GetData(fullAudioClipData, 0);

            Debug.LogError("Index: " + (currentMicPosition - this._samplesBetweenReads));

            if ((currentMicPosition - this._samplesBetweenReads) < 0)
            {
                this._currentLoudness = 0;
                Debug.LogError("It's 0");

            }
            else
            {
                Debug.LogError("It's something else");
                this._currentLoudness = fullAudioClipData[currentMicPosition - this._samplesBetweenReads];
            }

            yield return new WaitForSecondsRealtime(this._timeBetweenReadsInSeconds);
        }
    }

    /*
    
    private IEnumerator ReadInput()
    {
        float[] micData = new float[AudioSettings.outputSampleRate];
        double nextReadTime = AudioSettings.dspTime + this._timeBetweenReadsInSeconds;

        int currentPosition = 0;

        while (true)
        {
            if (AudioSettings.dspTime >= nextReadTime)
            {                           
                currentPosition = Microphone.GetPosition(this._device);

                Debug.LogError("Position: " + currentPosition);

                this._recordedAudioClip.GetData(micData, 0);
                this._currentLoudness = Mathf.Abs(micData[currentPosition]);

                Debug.LogError("Value: " + this._currentLoudness);

                nextReadTime = AudioSettings.dspTime + this._timeBetweenReadsInSeconds;
            }

            yield return null;
            
            /*
            //Clear last second of recorded audio after we reach the end of it
            if ((currentPosition + this._samplesBetweenReads) >= micData.Length - 1)
            {
                Array.Clear(micData, 0, AudioSettings.outputSampleRate);
                this._recordedAudioClip.SetData(micData, 0);
            }
            
}
    }

     private IEnumerator ReadInput()
    {
        while (true)
        {
            int currentPosition = Microphone.GetPosition(this._device);
            int startPosition = currentPosition - this._sampleWindow;
            float[] sampleData = new float[this._sampleWindow];

            if (startPosition < 0)
            {
                yield return null;
                continue;
            }

            this._recordedAudioClip.GetData(sampleData, startPosition);

            float totalLoudness = 0;

            for (int i = 0; i < _sampleWindow; i++)
            {
                totalLoudness += Mathf.Abs(sampleData[i]);
            }

            float averageLoudness = totalLoudness / this._sampleWindow;

            Debug.LogError("Loudness: " + averageLoudness);

            yield return null;
        }
    }
    
    
    
    private IEnumerator ReadInput()
    {
        float[] micData = new float[AudioSettings.outputSampleRate];
        double nextReadTime = AudioSettings.dspTime + this._timeBetweenReadsInSeconds;

        while (true)
        {
            if (AudioSettings.dspTime >= nextReadTime)
            {
                int currentPosition = Microphone.GetPosition(this._device);
                
                this._recordedAudioClip.GetData(micData, 0);
                float sampleValue = micData[currentPosition];

                Debug.LogError("Position: " + currentPosition + "\nValue: " + sampleValue);

                nextReadTime = AudioSettings.dspTime + this._timeBetweenReadsInSeconds;
            }

            yield return null;

            /*
            //Get data from the recorded audio every _timeBetweenReadsInMS 
            if ((currentPosition > 0) && (currentPosition % this._samplesBetweenReads == 0))
            {
                this._recordedAudioClip.GetData(micData, 0);

                float sampleValue = micData[currentPosition];

                Debug.LogError("Value at Position " + currentPosition + ": " + sampleValue);
            }

            //Clear last second of recorded audio after we reach the end of it
            if (currentPosition >= micData.Length - 1)
            {
                Array.Clear(micData, 0, AudioSettings.outputSampleRate);
                this._recordedAudioClip.SetData(micData, 0);
            }
                       
        }
        */
}
