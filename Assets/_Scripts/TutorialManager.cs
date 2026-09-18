using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource _tutorialMusic;

    [SerializeField]
    private AudioClip _flagClearedClip;

    private AudioChannelSettings _channelSettings;

    [SerializeField]
    private Transform _characterTransform;

    [SerializeField]
    private RectTransform _tutorialInstructions;

    private bool _lowFlagCleared = false;

    private bool _highFlagCleared = false;

    private float _buffer = 0.5f;

    private float _timeToGetToneInSeconds = 2.0f;

    private bool _startedToneCalibration = false;
    private float _elapsedTime = 0.0f;
    List<float> _currentPitches;

    private int _frameBuffer = 3;

    private void Awake()
    {
        this._channelSettings = new AudioChannelSettings(false, 1.0f, 1.0f, 0.5f, "SFX");
        this._characterTransform = GameObject.Find("PlayerCharacter").GetComponent<Transform>();

        this._currentPitches = new List<float>();

        StartCoroutine(this.GetLowTone());
    }

    private IEnumerator GetLowTone()
    {
        this.ResetToneCalibration();

        int currentFrameBuffer = 0;

        while (this._lowFlagCleared == false)
        {
            if (MicrophoneManager.instance.GetNormalizedLoudness() > MicrophoneManager.instance.noiseGate)
            {
                if (this._startedToneCalibration == false)
                {
                    this._startedToneCalibration = true;
                    this._characterTransform.DOKill();
                    this._characterTransform.DOMoveY(CharacterController._minYPosition, this._timeToGetToneInSeconds).SetEase(Ease.Linear);
                }

                //Debug.LogError(MicrophoneManager.instance.GetRawPitch());

                float rawPitch = MicrophoneManager.instance.GetRawPitch();


                if (this.IsPitchAnomalous(this._currentPitches, rawPitch) == false)
                {
                    this._currentPitches.Add(rawPitch);
                }
                this._elapsedTime += Time.fixedDeltaTime;

                if (this._elapsedTime > this._timeToGetToneInSeconds)
                {
                    this._lowFlagCleared = true;
                }
            }
            else
            {
                if (currentFrameBuffer >= this._frameBuffer)
                {
                    this.ResetToneCalibration();
                }
                else
                {
                    currentFrameBuffer++;
                }
            }

            yield return new WaitForFixedUpdate();
        }

        MicrophoneManager.instance.minClampedPitch = this.GetAveragePitch();
        Debug.LogError("Calibrated Min Pitch: " + MicrophoneManager.instance.minClampedPitch);

        AudioManager.instance.Play(this._flagClearedClip, this._channelSettings);

        StartCoroutine(this.GetHighTone());
    }

    private IEnumerator GetHighTone()
    {
        this.ResetToneCalibration();

        int currentFrameBuffer = 0;

        while (this._highFlagCleared == false)
        {
            if (MicrophoneManager.instance.GetNormalizedLoudness() > MicrophoneManager.instance.noiseGate)
            {
                if (this._startedToneCalibration == false)
                {
                    this._startedToneCalibration = true;
                    this._characterTransform.DOKill();
                    this._characterTransform.DOMoveY(CharacterController._maxYPosition, this._timeToGetToneInSeconds).SetEase(Ease.Linear);
                }

                //Debug.LogError(MicrophoneManager.instance.GetRawPitch());

                this._currentPitches.Add(MicrophoneManager.instance.GetRawPitch());

                this._elapsedTime += Time.fixedDeltaTime;

                if (this._elapsedTime > this._timeToGetToneInSeconds)
                {
                    this._highFlagCleared = true;
                }
            }
            else
            {
                if (currentFrameBuffer >= this._frameBuffer)
                {
                    this.ResetToneCalibration();
                }
                else
                {
                    currentFrameBuffer++;
                }
            }

            yield return new WaitForFixedUpdate();
        }

        MicrophoneManager.instance.maxClampedPitch = this.GetAveragePitch();
        Debug.LogError("Calibrated Max Pitch: " + MicrophoneManager.instance.maxClampedPitch);

        AudioManager.instance.Play(this._flagClearedClip, this._channelSettings);

        StartCoroutine(this.StartGame());
    }

    private IEnumerator StartGame()
    {
        this.ResetPlayerTransform();
        this._tutorialInstructions.DOScale(0.0f, 0.5f).SetEase(Ease.OutBack);

        yield return new WaitForSeconds(0.5f);

        this._tutorialMusic.Stop();

        GameManager.instance.StartGame();
    }

    private void ResetPlayerTransform()
    {
        this._characterTransform.DOKill();
        this._characterTransform.DOMoveY(0.0f, 0.5f).SetEase(Ease.OutBack);
    }

    private void ResetToneCalibration()
    {
        this._currentPitches.Clear();
        this._elapsedTime = 0.0f;
        this._startedToneCalibration = false;
        this.ResetPlayerTransform();
    }

    private float GetAveragePitch()
    {
        float total = 0.0f;

        for (int i = 0; i < this._currentPitches.Count; i++)
        {
            total += this._currentPitches[i];
        }

        return (total / this._currentPitches.Count);
    }

    private bool IsPitchAnomalous(List<float> recordedPitches, float rawPitch)
    {
        if (recordedPitches.Count < 5)
        {
            return false;
        }
    
        List<float> samplePitches = new List<float>();

        for (int i = 1; i <= 5; i++)
        {
            samplePitches.Add(recordedPitches[recordedPitches.Count - i]);
        }

        return MicrophoneManager.instance.IsPitchAnomalous(samplePitches, rawPitch);
    }

    /*
    private void Awake()
    {
        this._channelSettings = new AudioChannelSettings(false, 1.0f, 1.0f, 0.5f, "SFX");
        this._characterController = GameObject.Find("PlayerCharacter").GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this._lowFlagCleared && this._highFlagCleared)
        {
            this.EndTutorial();
        }

        if (this._lowFlagCleared == false && this._characterController.gameObject.transform.position.y <= (this._characterController._minYPosition + this._buffer))
        {
            this._lowFlagCleared = true;
            AudioManager.instance.Play(this._flagClearedClip, this._channelSettings);
        }

        if (this._highFlagCleared == false && this._characterController.gameObject.transform.position.y >= (this._characterController._maxYPosition - this._buffer))
        {
            this._highFlagCleared = true;
            AudioManager.instance.Play(this._flagClearedClip, this._channelSettings);
        }
    }

    private void EndTutorial()
    {
        this._tutorialMusic.Stop();
        MicrophoneManager.instance.StopMicInput();
        StartCoroutine(this.ResetPlayerPosition());
    }

    private IEnumerator ResetPlayerPosition()
    {
        this._characterController.gameObject.transform.DOMoveY(0.0f, 0.5f).SetEase(Ease.OutBack);
        this._tutorialInstructions.DOScale(0.0f, 0.5f).SetEase(Ease.OutBack);

        yield return new WaitForSeconds(0.5f);

        GameManager.instance.StartGame();

        Destroy(this.gameObject);
    }
    */


}