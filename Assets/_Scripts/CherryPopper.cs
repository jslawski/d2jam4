using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CherryPopper : MonoBehaviour
{
    public static CherryPopper instance;

    public Transform poppaCherry;

    public SpriteRenderer poppaSprite;

    public Sprite neutralSprite;
    public Sprite gasmSprite;

    public AudioClip[] _voicelines;

    private AudioChannelSettings _channelSettings;

    private float voicelineChance = 0.1f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        this._voicelines = Resources.LoadAll<AudioClip>("Voicelines");

        this._channelSettings = new AudioChannelSettings(false, 1.0f, 1.0f, 0.5f, "SFX");
    }

    public void Poppim()
    {
        StopAllCoroutines();
        StartCoroutine(this.PopCherry());
        this.RollForVoiceline();
    }

    public void RollForVoiceline()
    {
        float randomRoll = Random.Range(0.0f, 1.0f);

        if (randomRoll <= this.voicelineChance)
        {
            this.PlayRandomVoiceline();
        }
    }

    public void PopAndBark()
    { 
        StartCoroutine(this.PopCherry());
        PlayRandomVoiceline();
    }

    public void PlayRandomVoiceline()
    {
        int randomIndex = Random.Range(0, this._voicelines.Length);
        AudioManager.instance.Play(this._voicelines[randomIndex], this._channelSettings);
    }

    private IEnumerator PopCherry()
    {
        this.poppaSprite.sprite = gasmSprite;
        this.poppaCherry.DOShakePosition(0.25f, 0.2f);
        yield return new WaitForSeconds(1.0f);
        this.poppaSprite.sprite = neutralSprite;

    }
}
