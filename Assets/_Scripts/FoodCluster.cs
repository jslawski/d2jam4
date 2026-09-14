using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodCluster : MonoBehaviour
{
    [SerializeField]
    private float _secondsToReachDestination = 3.0f;
    [SerializeField]
    private float _secondsBetweenFoods = 2.0f;
    
    
    private float _beatsOfSeparation = 2.0f;
    

    private float _distancePerBeat = 0.0f;

    private float _minX;

    private float _moveSpeed;
    
    private Collider[] _allColliders;

    private float _targetXPosition;

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

        this._targetXPosition = targetXPosition;

        this.CalculateValues();
        
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

        if (minY < FoodSpawner.instance._minYSpawn)
        { 
            yAdjustment = FoodSpawner.instance._minYSpawn - minY;
        }

        if (maxY > FoodSpawner.instance._maxYSpawn)
        {
            yAdjustment = FoodSpawner.instance._maxYSpawn - maxY;
        }

        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + yAdjustment, this.transform.position.z);
    }

    private void CalculateValues()
    {
        float beatsToDestination = 12.0f;
        float beatsPerSecond = FoodSpawner.instance.songBPM / 60.0f;
        float secondsToDestination = beatsToDestination / beatsPerSecond;
        float distanceToDestination = Mathf.Abs(this._targetXPosition - this._allColliders[0].bounds.max.x);

        this._moveSpeed = distanceToDestination / this._secondsToReachDestination;
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

            //float distanceFromPreviousFood = ((2.0f * this._distancePerBeat * this._beatsOfSeparation) + currentColliderXExtents - (this._moveSpeed * Time.fixedDeltaTime));
            float distanceFromPreviousFood = (this._moveSpeed * this._secondsBetweenFoods) + currentColliderXExtents - (this._moveSpeed * Time.fixedDeltaTime);
            float newFoodXPosition = previousColliderXMin - distanceFromPreviousFood;

            Vector3 originalPosition = collider2.transform.parent.position;

            collider2.transform.parent.position = new Vector3(newFoodXPosition, originalPosition.y, originalPosition.z);

            yield return new WaitForFixedUpdate();
        }

        this.SetNextClusterSpawnTime();
    }

    private void SetNextClusterSpawnTime()
    {
        float distancePerBeat = this._moveSpeed / FoodSpawner.instance._beatsPerSecond;
        float xDistanceToTarget = Mathf.Abs(this._targetXPosition - this._allColliders[this._allColliders.Length - 1].bounds.max.x);

        float numBeatsToTarget = (xDistanceToTarget / distancePerBeat);

        FoodSpawner.instance.SetNextSpawnTime(numBeatsToTarget);
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
