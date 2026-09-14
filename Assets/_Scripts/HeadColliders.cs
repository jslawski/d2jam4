using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadColliders : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (MouthController.instance._currentFood != null)
        {
            MouthController.instance.DropFood();
        }
        else
        {
            other.gameObject.GetComponentInParent<FoodObject>().BounceFood();
        }
    }
}
