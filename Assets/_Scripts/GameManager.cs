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
    public GameTimer _timer;

    [SerializeField]
    private GameObject _optionsCanvas;

    [SerializeField]
    private GameObject _tutorialManagerPrefab;

    [SerializeField]
    private GameObject _characterControllerObject;

    [SerializeField]
    private GameObject _transitionObject;

    [SerializeField]
    private GameObject _endScreen;

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
        if (Input.GetKeyUp(KeyCode.R))
        {
            SceneManager.LoadScene("JaredScene");     
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            SceneLoader.instance.LoadScene("MainMenu");
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
        CherryPopper.instance.PopAndBark();
        this._characterControllerObject.GetComponent<CharacterController>().isTutorial = false;
    }

    public void EndGame()
    {        
        this._timer.StopTimer();
        FoodSpawner.instance.StopFoodSpawning();
        FoodSpawner.instance.DestroyAllFood();
        StartCoroutine(EndSequence());
    }

    private IEnumerator EndSequence()
    {
        this._transitionObject.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        this._endScreen.SetActive(true);
    }

    public float GetTime()
    {
        return this._timer.GetRawTime();
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
