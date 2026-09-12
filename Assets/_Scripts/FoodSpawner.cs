using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public AudioSource _musicSource;

    public static int songBPM = 128;
    
    private float _beatsPerSecond;
    private float _samplesPerBeat;

    private double _previousBeatTimeInSamples;

    // Start is called before the first frame update
    void Start()
    {
        this._beatsPerSecond = FoodSpawner.songBPM / 60.0f;
        this._samplesPerBeat = Mathf.FloorToInt(AudioSettings.outputSampleRate / this._beatsPerSecond);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            this.StartFoodSpawning();
        }
    }

    public void StartFoodSpawning()
    {
        this._musicSource.Play();
        StartCoroutine(this.SpawnLogic());
    }

    private IEnumerator SpawnLogic()
    {
        this._previousBeatTimeInSamples = this.GetCurrentSampleTime();

        while (true)
        {
            //Debug.LogError(AudioSettings.dspTime - this._previousBeatTime);
            if ((this.GetCurrentSampleTime() - this._previousBeatTimeInSamples >= this._samplesPerBeat))
            {
                this._previousBeatTimeInSamples = this.GetCurrentSampleTime();
                this.SpawnFood();
            }
        
            yield return null;
        }
    }

    private double GetCurrentSampleTime()
    {
        return (AudioSettings.dspTime * AudioSettings.outputSampleRate);
    }

    private void SpawnFood()
    {
        Debug.LogError("SPAWN!");
    }
}
