using System.Collections;
using System.Collections.Generic;
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

    private GameObject[] _allFoods;

    [SerializeField]
    private Transform _playerTransform;

    [SerializeField]
    private GameObject _optionsCanvas;

    private float _playerXPosition;

    private float _nextSpawnTimeInSamples;

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

        this._allFoods = Resources.LoadAll<GameObject>("FoodClusters/Test");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.S))
        {
            this.StartFoodSpawning();
        }

        if (Input.GetKeyUp(KeyCode.O))
        {
            this._optionsCanvas.SetActive(!this._optionsCanvas.activeSelf);
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }

    public void StartFoodSpawning()
    {
        this._musicSource.Play();
        StartCoroutine(this.SpawnLogic());
    }

    public void SetNextSpawnTime(float numBeats)
    {
        this._nextSpawnTimeInSamples = numBeats * this._samplesPerBeat;
    }

    private IEnumerator SpawnLogic()
    {
        this._previousBeatTimeInSamples = this.GetCurrentSampleTime();

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
        Vector3 spawnPosition = new Vector3(this._xSpawn, Random.Range(this._minYSpawn, this._maxYSpawn), this.transform.position.z);

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
