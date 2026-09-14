using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [SerializeField]
    private AudioSource[] _audioSources;

    private int _currentIndex = 0;

    private float _maxVolume = 0.3f;

    private bool _gameStarted = false;

    private void Awake()
    {
        this._audioSources = GetComponents<AudioSource>();

        if (instance == null)
        {
            instance = this;
        }
    }

    private void FixedUpdate()
    {
        if (this._gameStarted == false)
        {
            return;
        }

        this.UpdateMusic();
    }

    public void StartMusic()
    {
        this._audioSources[0].volume = this._maxVolume;
        this._audioSources[0].Play();
    
        for (int i = 1; i < this._audioSources.Length; i++)
        {
            this._audioSources[i].volume = 0.0f;    
            this._audioSources[i].Play();
        }
    }

    public void UpdateMusic()
    {
        float currentPlaytime = GameManager.instance.GetCurrentPlaytimeInSeconds();
        bool shouldCrossFade = false;

        if (this._currentIndex == 4 && currentPlaytime > 112.5)
        {
            shouldCrossFade = true;
        }
        else if (this._currentIndex == 3 && currentPlaytime > 90)
        {
            shouldCrossFade = true;
        }
        else if (this._currentIndex == 2 && currentPlaytime > 67.5)
        {
            shouldCrossFade = true;
        }
        else if (this._currentIndex == 1 && currentPlaytime > 45)
        {
            shouldCrossFade = true;
        }
        else if (this._currentIndex == 0 && currentPlaytime > 22.5f)
        {
            shouldCrossFade = true;
        }

        if (shouldCrossFade == true)
        {
            this._currentIndex++;
            this.CrossFade(this._audioSources[this._currentIndex - 1], this._audioSources[this._currentIndex]);
        }
    }

    private void CrossFade(AudioSource fadeOutSource, AudioSource fadeInSource)
    {
        Debug.LogError("Fading Out " + fadeOutSource.clip.name + "\nFading In: " + fadeInSource.clip.name);
    
        float fadeOutValue = fadeOutSource.volume;
        DOTween.To(() => fadeOutValue, x => fadeOutValue = x, 0.0f, 0.3f)
            .OnUpdate(() => {
                fadeOutSource.volume = fadeOutValue;
            });

        float fadeInValue = 0.0f;
        DOTween.To(() => fadeInValue, x => fadeInValue = x, this._maxVolume, 0.3f)
            .OnUpdate(() => {
                fadeInSource.volume = fadeInValue;
            });
    }
}
