using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlane : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.GetComponent<FoodObject>().ImmediatelyDestroyFood();
        //AUDIO: Food Miss
    }
}
