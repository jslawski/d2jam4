using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FoodSpawner : MonoBehaviour
{
    public static FoodSpawner instance;    

    public AudioSource _musicSource;

    public int songBPM = 128;

    private float _xSpawn;
    public float _minYSpawn;
    public float _maxYSpawn;

    public float _beatsPerSecond;
    private float _samplesPerBeat;

    private double _previousBeatTimeInSamples;

    private GameObject[] _easyFoodClusters;
    private GameObject[] _mediumFoodClusters;
    private GameObject[] _hardFoodClusters;

    [SerializeField]
    private Transform _playerTransform;

    private float _playerXPosition;

    private float _nextSpawnTimeInSamples;

    [SerializeField]
    private TextMeshProUGUI _debugDifficultyText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        this.SetupSpawnParameters();
        this._playerXPosition = this._playerTransform.position.x;
    }

    // Start is called before the first frame update
    void Start()
    {
        this._beatsPerSecond = this.songBPM / 60.0f;
        this._samplesPerBeat = Mathf.FloorToInt(AudioSettings.outputSampleRate / this._beatsPerSecond);

        this._easyFoodClusters = Resources.LoadAll<GameObject>("FoodClusters/01_Easy");
        this._mediumFoodClusters = Resources.LoadAll<GameObject>("FoodClusters/02_Medium");
        this._hardFoodClusters = Resources.LoadAll<GameObject>("FoodClusters/03_Hard");
    }

    public void StartFoodSpawning()
    {
        this._musicSource.Play();
        StartCoroutine(this.SpawnLogic());
    }

    public void StopFoodSpawning()
    {
        StopAllCoroutines();
    }

    public void SetNextSpawnTime(float numBeats)
    {
        this._nextSpawnTimeInSamples = numBeats * this._samplesPerBeat;
    }

    private IEnumerator SpawnLogic()
    {
        this._previousBeatTimeInSamples = this.GetCurrentSampleTime();

        this.SetNextSpawnTime(4.0f);

        while (true)
        {
            if ((this.GetCurrentSampleTime() - this._previousBeatTimeInSamples >= this._nextSpawnTimeInSamples))
            {
                this._previousBeatTimeInSamples = this.GetCurrentSampleTime();
                this._nextSpawnTimeInSamples = float.PositiveInfinity;
                this.SpawnFoodCluster();
            }
        
            yield return null;
        }
    }

    private void SetupSpawnParameters()
    {
        BoxCollider spawnZone = this.GetComponent<BoxCollider>();
        this._xSpawn = spawnZone.gameObject.transform.position.x;
        this._minYSpawn = spawnZone.bounds.min.y;
        this._maxYSpawn = spawnZone.bounds.max.y;
    }

    private double GetCurrentSampleTime()
    {
        return (AudioSettings.dspTime * AudioSettings.outputSampleRate);
    }

    private void SpawnFoodCluster()
    {
        DifficultyScaler.UpdateDifficulty();

        this._debugDifficultyText.text = DifficultyScaler.currentDifficulty.ToString();

        Vector3 spawnPosition = new Vector3(this._xSpawn, Random.Range(this._minYSpawn, this._maxYSpawn), this.transform.position.z);

        GameObject spawnedFood = Instantiate(this.GetRandomFood(), spawnPosition, new Quaternion(), this.transform);
        FoodCluster foodComponent = spawnedFood.GetComponent<FoodCluster>();
        foodComponent.InitializeCluster(this._playerXPosition);
    }

    private GameObject GetRandomFood()
    {
        GameObject[] chosenBucket = this.GetDifficultyBucket();

        int randomIndex = Random.Range(0, chosenBucket.Length);
        return chosenBucket[randomIndex];
    }

    private GameObject[] GetDifficultyBucket()
    {
        float randomRoll = Random.Range(0.0f, 1.0f);
        float majority = 0.8f;

        if (DifficultyScaler.currentDifficulty == Difficulty.VERYHARD)
        {
            return this._hardFoodClusters;
        }
        else if (DifficultyScaler.currentDifficulty == Difficulty.HARD)
        {
            if (randomRoll <= majority)
            {
                return this._hardFoodClusters;
            }
            else
            {
                return this._mediumFoodClusters;
            }
        }
        else if (DifficultyScaler.currentDifficulty == Difficulty.MEDIUMHARD)
        {
            if (randomRoll <= majority)
            {
                return this._mediumFoodClusters;
            }
            else
            {
                return this._hardFoodClusters;
            }
        }
        else if (DifficultyScaler.currentDifficulty == Difficulty.MEDIUM)
        {
            if (randomRoll <= majority)
            {
                return this._mediumFoodClusters;
            }
            else
            {
                return this._easyFoodClusters;
            }
        }
        else if (DifficultyScaler.currentDifficulty == Difficulty.EASYMEDIUM)
        {
            if (randomRoll <= majority)
            {
                return this._easyFoodClusters;
            }
            else
            {
                return this._mediumFoodClusters;
            }
        }
        else
        {
            return this._easyFoodClusters;
        }
    }
}
