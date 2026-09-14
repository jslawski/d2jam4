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

        AudioManager.instance.Play(this._missClip, this._channelSettings);

        Invoke("DestroyAfterDelay", 2.0f);
    }

    public void Hide()
    {
        this._foodCollider.enabled = false;
        this._foodRenderer.enabled = false;
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

/*
    private List<float> _beatOptions = new List<float> { 4.0f, 6.0f, 8.0f };    
    public void LaunchFood(float targetXPosition)
    {
        float beatsToDestination = 4.0f;// this.GetRandomBeatsToDestination();
        float beatsPerSecond = FoodSpawner.instance.songBPM / 60.0f;
        float secondsToDestination = beatsToDestination / beatsPerSecond;
        float distanceToDestination = Mathf.Abs(targetXPosition - this.GetBoundXValue());

        float targetSpeed = distanceToDestination / secondsToDestination;

        StartCoroutine(MoveFood(targetSpeed));
    }

    private IEnumerator MoveFood(float moveSpeed)
    {        
        while (true)
        {
            this.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        
            yield return null;
        }
    }

    private float GetRandomBeatsToDestination()
    {
        int randomIndex = Random.Range(0, this._beatOptions.Count);
        return this._beatOptions[randomIndex];
    }

    private float GetBoundXValue()
    {
        return this._foodCollider.bounds.max.x;
    }*/