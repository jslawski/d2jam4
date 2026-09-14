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
    private CharacterController _characterController;

    [SerializeField]
    private RectTransform _tutorialInstructions;

    private bool _lowFlagCleared = false;

    private bool _highFlagCleared = false;

    private float _buffer = 0.5f;

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
}
