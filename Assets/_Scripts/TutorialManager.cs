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

    private bool _lowFlagCleared = false;

    private bool _highFlagCleared = false;

    private float _buffer = 0.3f;

    private void Awake()
    {
        this._channelSettings = new AudioChannelSettings(false, 1.0f, 1.0f, 0.5f, "SFX");
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
            Debug.LogError("Low Flag Cleared!");
            AudioManager.instance.Play(this._flagClearedClip, this._channelSettings);
        }

        if (this._highFlagCleared == false && this._characterController.gameObject.transform.position.y >= (this._characterController._maxYPosition - this._buffer))
        {
            this._highFlagCleared = true;
            Debug.LogError("High Flag Cleared!");
            AudioManager.instance.Play(this._flagClearedClip, this._channelSettings);
        }
    }

    private void EndTutorial()
    {
        this._tutorialMusic.Stop();
        GameManager.instance.StartGame();
        Destroy(this.gameObject);
    }
}
