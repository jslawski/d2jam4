using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodObject : MonoBehaviour
{
    private Rigidbody _foodRb;

    public int pointsValue = 10;

    private Vector3 _bounceVector;

    private Collider _foodCollider;
    private Renderer _foodRenderer;

    public bool isBeingEaten = false;

    private AudioChannelSettings _channelSettings;
    private AudioClip _missClip;

    [SerializeField]
    private GameObject _splatParticlePrefab;

    private void Awake()
    {
        this._bounceVector = new Vector3(-0.75f, 1.0f, 0.0f).normalized;

        this._foodRb = GetComponent<Rigidbody>();
        this._foodCollider = GetComponentInChildren<Collider>();
        this._foodRenderer = GetComponentInChildren<Renderer>();

        this._channelSettings = new AudioChannelSettings(false, 0.9f, 1.1f, 1.0f, "SFX");
        this._missClip = Resources.Load<AudioClip>("Audio/miss");
    }

    public void BounceFood()
    {
        this.transform.parent = null;
        this._foodRb.isKinematic = false;
        this._foodRb.useGravity = true;

        this._foodCollider.enabled = false;

        float randomMagnitude = Random.Range(4.0f, 6.0f); ;
        Vector3 randomDirection = Random.onUnitSphere;

        this._foodRb.AddForce(this._bounceVector * randomMagnitude, ForceMode.Impulse);
        this._foodRb.AddTorque(randomDirection * randomMagnitude, ForceMode.Impulse);

        this.isBeingEaten = false;

        GameManager.instance.RemoveHealth();

        Instantiate(this._splatParticlePrefab, this.transform.position, new Quaternion());

        AudioManager.instance.Play(this._missClip, this._channelSettings);

        CharacterController.Jostle();

        Invoke("DestroyAfterDelay", 2.0f);
    }

    public void Hide()
    {    
        this.transform.parent = null;
        this._foodCollider.enabled = false;
        this.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InOutBack);
    }

    public void EatFood()
    {
        Destroy(this.gameObject);
    }

    public void ImmediatelyDestroyFood()
    {
        AudioManager.instance.Play(this._missClip, this._channelSettings);

        GameManager.instance.RemoveHealth();

        Destroy(this.gameObject);        
    }

    private void DestroyAfterDelay()
    {
        Destroy(this.gameObject);
    }
}