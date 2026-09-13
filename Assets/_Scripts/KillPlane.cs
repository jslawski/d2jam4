using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlane : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        FoodObject foodObject = other.gameObject.GetComponent<FoodObject>();

        if (foodObject.isBeingEaten == false)
        {
            foodObject.ImmediatelyDestroyFood();
        }
    }
}
