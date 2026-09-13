using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodCluster : MonoBehaviour
{
    [SerializeField]
    private float _beatsOfSeparation = 2.0f;

    private float _distancePerBeat = 0.0f;

    private float _minX;

    private float _moveSpeed;
    
    private Collider[] _allColliders;

    //Get all colliders in cluster
    //Arrange the foods in order based on their order in the list (top food is furthest right)
    //The food's Y positions can be preserved
    //Spread foods out so that they are "1 beat" separated from each other
    //Use the first food's maxX to determine speed
    //Use the last foods minX to determine when the next cluster should spawn
    //You have to use min and max Y bounds to make sure the whole cluster fits on screen
    public void InitializeCluster(float targetXPosition)
    { 
        this._allColliders = GetComponentsInChildren<Collider>();

        this.AdjustVerticalPosition();

        this.CalculateValues(targetXPosition);
        
        StartCoroutine(this.SpaceOutFood());

        StartCoroutine(this.MoveFood());
    }

    private void AdjustVerticalPosition()
    {
        float minY = float.PositiveInfinity;
        float maxY = float.NegativeInfinity;

        for (int i = 0; i < this._allColliders.Length; i++)
        {
            Bounds currentBounds = this._allColliders[i].bounds;
            if (currentBounds.max.y > maxY)
            {
                maxY = currentBounds.max.y;
            }
            if (currentBounds.min.y < minY)
            {
                minY = currentBounds.min.y;
            }
        }

        float yAdjustment = 0.0f;

        if (minY < FoodSpawner._minYSpawn)
        { 
            yAdjustment = FoodSpawner._minYSpawn - minY;
        }

        if (maxY > FoodSpawner._maxYSpawn)
        {
            yAdjustment = FoodSpawner._maxYSpawn - maxY;
        }

        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + yAdjustment, this.transform.position.z);
    }

    private void CalculateValues(float targetXPosition)
    {
        float beatsToDestination = 12.0f;
        float beatsPerSecond = FoodSpawner.songBPM / 60.0f;
        float secondsToDestination = beatsToDestination / beatsPerSecond;
        float distanceToDestination = Mathf.Abs(targetXPosition - this._allColliders[0].bounds.max.x);

        this._moveSpeed = distanceToDestination / secondsToDestination;
        this._distancePerBeat = this._moveSpeed / beatsPerSecond;
    }    

    private IEnumerator SpaceOutFood()
    {
        for (int i = 1; i < this._allColliders.Length; i++)
        {
            Collider collider1 = this._allColliders[i - 1];
            Collider collider2 = this._allColliders[i];

            float previousColliderXMin = collider1.bounds.min.x;
            float currentColliderXExtents = collider2.bounds.extents.x;            

            float distanceFromPreviousFood = ((2.0f * this._distancePerBeat * this._beatsOfSeparation) + currentColliderXExtents - (this._moveSpeed * Time.fixedDeltaTime));
            float newFoodXPosition = previousColliderXMin - distanceFromPreviousFood;

            Vector3 originalPosition = collider2.transform.position;

            collider2.transform.position = new Vector3(newFoodXPosition, originalPosition.y, originalPosition.z);

            yield return new WaitForFixedUpdate();
        }        
    }

    private IEnumerator MoveFood()
    {
        while (true)
        {
            this.transform.Translate(Vector3.right * this._moveSpeed * Time.deltaTime);

            yield return null;
        }
    }
}
