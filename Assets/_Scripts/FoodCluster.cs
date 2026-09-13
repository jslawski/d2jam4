using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodCluster : MonoBehaviour
{
    [SerializeField]
    private float _beatsOfSeparation = 0.5f;

    private float _distancePerBeat = 0.0f;

    private float _minX;
    private float _minY;

    private float _maxX;
    private float _maxY;

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

        this.CalculateValues(targetXPosition);
        
        StartCoroutine(this.SpaceOutFood());

        StartCoroutine(this.MoveFood());
    }

    private void CalculateValues(float targetXPosition)
    {
        float beatsToDestination = 4.0f;
        float beatsPerSecond = FoodSpawner.songBPM / 60.0f;
        float secondsToDestination = beatsToDestination / beatsPerSecond;
        float distanceToDestination = Mathf.Abs(targetXPosition - this._allColliders[0].bounds.max.x);

        this._moveSpeed = distanceToDestination / secondsToDestination;
        this._distancePerBeat = this._moveSpeed / beatsPerSecond;
    }

    
    private IEnumerator SpaceOutFood()
    {
    /*   
    //First, unparent all children
        for (int i = 0; i < this._allColliders.Length; i++)
        {
            this._allColliders[i].transform.parent = null;
        }
        */
        for (int i = 1; i < this._allColliders.Length; i++)
        {
            Collider collider1 = this._allColliders[i - 1];
            Collider collider2 = this._allColliders[i];

            Debug.LogError("Collider1: " + collider1.gameObject.name + "\nCollider2: " + collider2.gameObject.name);

            float previousColliderXMin = collider1.bounds.min.x;
            float currentColliderXExtents = collider2.bounds.extents.x;            

            float distanceFromPreviousFood = ((2.0f * this._distancePerBeat * this._beatsOfSeparation) + currentColliderXExtents - (this._moveSpeed * Time.fixedDeltaTime));
            float newFoodXPosition = previousColliderXMin - distanceFromPreviousFood;

            Vector3 originalPosition = collider2.transform.position;

            collider2.transform.position = new Vector3(newFoodXPosition, originalPosition.y, originalPosition.z);

            yield return new WaitForFixedUpdate();
        }
        /*
        //Finally, re-parent all children again
        for (int i = 0; i < this._allColliders.Length; i++)
        {
            this._allColliders[i].transform.parent = this.transform;
        }
        */
        
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
