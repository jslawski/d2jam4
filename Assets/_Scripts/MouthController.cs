using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MouthController : MonoBehaviour
{
    public static MouthController instance;

    public FoodObject _currentFood;

    private bool _readyToSwallow;

    private bool _mouthIsOpen = false;

    [SerializeField]
    private Transform _mouthColliderTransform;
    [SerializeField]
    private Transform _topColliderTransform;
    [SerializeField]
    private Transform _bottomColliderTransform;

    private float _mouthMaxScale = 0.65f;

    [SerializeField]
    private Vector3 _topClosedPosition;
    [SerializeField]
    private Vector3 _topOpenPosition;

    [SerializeField]
    private Vector3 _bottomClosedPosition;
    [SerializeField]
    private Vector3 _bottomOpenPosition;

    private AudioChannelSettings _channelSettings;

    [SerializeField]
    private AudioClip _swallowReadyClip;
    [SerializeField]
    private AudioClip _swallowClip;
    [SerializeField]
    private AudioClip _chokeClip;

    [SerializeField]
    private Material _neckMaterial;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        this._channelSettings = new AudioChannelSettings(false, 0.9f, 1.1f, 0.5f, "SFX");
    }

    private void Update()
    {
        float normalizedLoudness = MicrophoneManager.instance.GetNormalizedLoudness();
        
        if (normalizedLoudness > MicrophoneManager.instance.noiseGate)
        {
            this._mouthIsOpen = true;
        }
        else if (this._mouthIsOpen == true)
        {
            this.AttemptSwallow();
        }
    }

    private void OnTriggerEnter(Collider other)
    {        
        if (this._currentFood == null && this._mouthIsOpen == true)
        {
            this._currentFood = other.gameObject.GetComponentInParent<FoodObject>();
            this._currentFood.isBeingEaten = true;
        }
        else
        {
            other.gameObject.GetComponentInParent<FoodObject>().BounceFood();
            AudioManager.instance.Play(this._chokeClip, this._channelSettings);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponentInParent<FoodObject>() == this._currentFood)
        {
            this._readyToSwallow = true;
            AudioManager.instance.Play(this._swallowReadyClip, this._channelSettings);
            this._currentFood.Hide();
        }
    }

    public void DropFood()
    {
        if (this._currentFood != null)
        {
            this._currentFood.BounceFood();
            this._currentFood = null;
            this._readyToSwallow = false;
        }
    }

    private void AttemptSwallow()
    {
        if (this._currentFood != null)
        {
            if (this._readyToSwallow == true)
            {
                this._readyToSwallow = false;
                this._currentFood = null;
                this.AnimateNeckSwallow();

                GameManager.instance.AddHealth();

                AudioManager.instance.Play(this._swallowClip, this._channelSettings);
            }
            else
            {
                this._currentFood.BounceFood();
                this._currentFood = null;
                AudioManager.instance.Play(this._chokeClip, this._channelSettings);
            }
        }

        this._mouthIsOpen = false;
    }

    private void AnimateNeckSwallow()
    {
        float currentValue = 0.0f;
        DOTween.To(() => currentValue, x => currentValue = x, 0.5f, 0.5f)
            .OnUpdate(() => {
                this._neckMaterial.SetFloat("_Bulge_Move", currentValue);
            });
    }

    public void UpdateFaceColliders(float normalizedLoudness)
    {        
        float newYScale = Mathf.Lerp(0.0f, this._mouthMaxScale, normalizedLoudness);
        this._mouthColliderTransform.localScale = new Vector3(this._mouthColliderTransform.localScale.x, newYScale, this._mouthColliderTransform.localScale.z);

        this._topColliderTransform.localPosition = Vector3.Lerp(this._topClosedPosition, this._topOpenPosition, normalizedLoudness);
        this._bottomColliderTransform.localPosition = Vector3.Lerp(this._bottomClosedPosition, this._bottomOpenPosition, normalizedLoudness);
    }
}
