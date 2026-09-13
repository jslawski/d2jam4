using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodObject : MonoBehaviour
{
    private List<float> _beatOptions = new List<float> { 4.0f, 6.0f, 8.0f };
    
    [SerializeField]
    private Collider _foodCollider;

    public void LaunchFood(float targetXPosition)
    {
        float beatsToDestination = 4.0f;// this.GetRandomBeatsToDestination();
        float beatsPerSecond = FoodSpawner.songBPM / 60.0f;
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
    }
}
