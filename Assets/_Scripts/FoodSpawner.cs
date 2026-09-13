using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public AudioSource _musicSource;

    public static int songBPM = 128;

    private float _xSpawn;
    public static float _minYSpawn;
    public static float _maxYSpawn;

    private float _beatsPerSecond;
    private float _samplesPerBeat;

    private double _previousBeatTimeInSamples;

    private GameObject[] _allFoods;

    [SerializeField]
    private Transform _playerTransform;

    private float _playerXPosition;

    private void Awake()
    {
        this.SetupSpawnParameters();
        this._playerXPosition = this._playerTransform.position.x;
    }

    // Start is called before the first frame update
    void Start()
    {
        this._beatsPerSecond = FoodSpawner.songBPM / 60.0f;
        this._samplesPerBeat = Mathf.FloorToInt(AudioSettings.outputSampleRate / this._beatsPerSecond);

        this._allFoods = Resources.LoadAll<GameObject>("FoodClusters");
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

        float derp = this._samplesPerBeat * 4.0f;

        while (true)
        {
            if ((this.GetCurrentSampleTime() - this._previousBeatTimeInSamples >= derp))
            {
                this._previousBeatTimeInSamples = this.GetCurrentSampleTime();
                this.SpawnFoodCluster();
            }
        
            yield return null;
        }
    }

    private void SetupSpawnParameters()
    {
        BoxCollider spawnZone = this.GetComponent<BoxCollider>();
        this._xSpawn = spawnZone.gameObject.transform.position.x;
        FoodSpawner._minYSpawn = spawnZone.bounds.min.y;
        FoodSpawner._maxYSpawn = spawnZone.bounds.max.y;
    }

    private double GetCurrentSampleTime()
    {
        return (AudioSettings.dspTime * AudioSettings.outputSampleRate);
    }

    private void SpawnFood()
    {
        Vector3 spawnPosition = new Vector3(this._xSpawn, Random.Range(FoodSpawner._minYSpawn, FoodSpawner._maxYSpawn), 0.0f);

        GameObject spawnedFood = Instantiate(this.GetRandomFood(), spawnPosition, new Quaternion(), this.transform);
        FoodObject foodComponent = spawnedFood.GetComponent<FoodObject>();
        foodComponent.LaunchFood(this._playerXPosition);
    }

    private void SpawnFoodCluster()
    {
        Vector3 spawnPosition = new Vector3(this._xSpawn, Random.Range(FoodSpawner._minYSpawn, FoodSpawner._maxYSpawn), 0.0f);

        GameObject spawnedFood = Instantiate(this.GetRandomFood(), spawnPosition, new Quaternion(), this.transform);
        FoodCluster foodComponent = spawnedFood.GetComponent<FoodCluster>();
        foodComponent.InitializeCluster(this._playerXPosition);
    }

    private GameObject GetRandomFood()
    {        
        int randomIndex = Random.Range(0, this._allFoods.Length);
        return this._allFoods[randomIndex];
    }
}
