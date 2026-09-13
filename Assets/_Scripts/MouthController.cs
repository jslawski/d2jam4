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

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
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
        if (this._currentFood == null)
        {
            this._currentFood = other.gameObject.GetComponent<FoodObject>();
            //AUDIO: Food Obtained
        }
        else
        {
            other.gameObject.GetComponent<FoodObject>().BounceFood();
            this._readyToSwallow = false;
            //AUDIO: Choking
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<FoodObject>() == this._currentFood)
        {
            this._readyToSwallow = true;
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
                //TODO: Add points and other feedback here
                //AUDIO: Swallow
            }
            else
            {
                this._currentFood.BounceFood();
                this._currentFood = null;
                //AUDIO: Choking
            }
        }

        this._mouthIsOpen = false;
    }

    public void UpdateFaceColliders(float normalizedLoudness)
    {        
        float newYScale = Mathf.Lerp(0.0f, this._mouthMaxScale, normalizedLoudness);
        this._mouthColliderTransform.localScale = new Vector3(this._mouthColliderTransform.localScale.x, newYScale, this._mouthColliderTransform.localScale.z);

        this._topColliderTransform.localPosition = Vector3.Lerp(this._topClosedPosition, this._topOpenPosition, normalizedLoudness);
        this._bottomColliderTransform.localPosition = Vector3.Lerp(this._bottomClosedPosition, this._bottomOpenPosition, normalizedLoudness);
    }
}
