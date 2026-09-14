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
            SceneManager.LoadScene(0);
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
}
