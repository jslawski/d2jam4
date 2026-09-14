using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField]
    private HealthBar _healthBar;
    [SerializeField]
    private GameTimer _timer;

    [SerializeField]
    private GameObject _optionsCanvas;

    [SerializeField]
    private GameObject _tutorialManagerPrefab;

    [SerializeField]
    private GameObject _characterControllerObject;

    public float currentHealth = 1.0f;

    public int foodsEaten = 0;

    private int _currentStreak = 0;
    public int highestStreak = 0;

    private float _healthPerMiss = 0.10f;
    private float _healthPerSwallow = 0.03f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.S))
        {
            this.StartGame();
        }

        if (Input.GetKeyUp(KeyCode.O))
        {
            this._optionsCanvas.SetActive(!this._optionsCanvas.activeSelf);
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            this.ResetGame();    
        
            //MicrophoneManager.instance.ClearAudioData();
            
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            this.RemoveHealth();
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            this.AddHealth();
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            this.EndGame();
        }
    }

    public void AddHealth()
    {
        this._healthBar.AddHealth(this._healthPerSwallow);
        this.foodsEaten++;
        this._currentStreak++;

        if (this._currentStreak > highestStreak)
        {
            this.highestStreak = this._currentStreak;
        }


    }

    public void RemoveHealth()
    {
        this._healthBar.RemoveHealth(this._healthPerMiss);
        this._currentStreak = 0;
    }

    public void StartGame()
    {
        this._timer.StartTimer();
        MusicManager.instance.StartMusic();
        FoodSpawner.instance.StartFoodSpawning();
        MicrophoneManager.instance.ActivateMicInput();
    }

    public void EndGame()
    {
        return;
        this._timer.StopTimer();
        FoodSpawner.instance.StopFoodSpawning();
        MicrophoneManager.instance.StopMicInput();

        //Display End Screen Here
    }

    public float GetCurrentPlaytimeInSeconds()
    {
        return this._timer.GetRawTime();
    }

    public void ResetGame()
    {
        //Player Position    
        this._characterControllerObject.transform.position = new Vector3(this._characterControllerObject.transform.position.x, 0.0f, this._characterControllerObject.transform.position.z);
        //Score
        this.foodsEaten = 0;
        this.highestStreak = 0;
        this._currentStreak = 0;
        //Health
        this.currentHealth = 1.0f;
        this.AddHealth();
        //Timer
        this._timer.ResetTimer();
        //Music
        MusicManager.instance.StopMusic();
        MusicManager.instance.StartMusic();
        //Difficulty
        DifficultyScaler.currentDifficulty = Difficulty.EASY;
        //Re-enable mic input
        MicrophoneManager.instance.ActivateMicInput();
        //Destroy All Food
        FoodSpawner.instance.DestroyAllFood();

    }
}
