using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    [SerializeField]
    private RectTransform _rectTransform;

    [SerializeField]
    private TextMeshProUGUI _latestFoodsEaten;
    [SerializeField]
    private TextMeshProUGUI _latestBestStreak;
    [SerializeField]
    private TextMeshProUGUI _latestTime;
    [SerializeField]
    private TextMeshProUGUI _bestFoodsEaten;
    [SerializeField]
    private TextMeshProUGUI _bestBestStreak;
    [SerializeField]
    private TextMeshProUGUI _bestTime;

    private void OnEnable()
    {    
        int latestFoodsEaten = GameManager.instance.foodsEaten;
        int latestStreak = GameManager.instance.highestStreak;
        float latestTime = GameManager.instance._timer.GetRawTime();

        int foodsEatenPB = PlayerPrefs.GetInt("foodEaten", GameManager.instance.foodsEaten);
        int streakPB = PlayerPrefs.GetInt("streak", GameManager.instance.highestStreak);
        float timePB = PlayerPrefs.GetFloat("bestTime", GameManager.instance._timer.GetRawTime());

        this._latestFoodsEaten.text = latestFoodsEaten.ToString();
        this._latestBestStreak.text = latestStreak.ToString();
        this._latestTime.text = GameManager.instance._timer.GetCurrentTimerString();

        if (latestFoodsEaten >= foodsEatenPB)
        {
            PlayerPrefs.SetInt("foodEaten", latestFoodsEaten);
            foodsEatenPB = latestFoodsEaten;
        }

        if (latestStreak >= streakPB)
        {
            PlayerPrefs.SetInt("streak", latestStreak);
            streakPB = latestStreak;
        }

        if (latestTime >= timePB)
        {
            PlayerPrefs.SetFloat("bestTime", latestTime);
            timePB = latestTime;
        }

        this._bestFoodsEaten.text = foodsEatenPB.ToString();
        this._bestBestStreak.text = streakPB.ToString();
        this._bestTime.text = GameManager.instance._timer.ConvertTimeToString(timePB);

        this._rectTransform.DOScale(1.0f, 0.5f).SetEase(Ease.OutBack);
    }

    public void RetryPressed()
    {
        SceneLoader.instance.LoadScene("JaredScene");
    }

    public void MenuPressed()
    {
        SceneLoader.instance.LoadScene("MainMenu");        
    }
}
