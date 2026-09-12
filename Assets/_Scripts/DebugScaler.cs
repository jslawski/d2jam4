using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScaler : MonoBehaviour
{
    private float scaleMultiplier = 10;

    public Vector3 minScale;
    public Vector3 maxScale;

    public Vector3 topPosition;
    public Vector3 bottomPosition;

    public Vector3 targetPosition = Vector3.zero;
    public float moveSpeed = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        this.minScale = Vector3.one * 0.5f;
        this.maxScale = Vector3.one * 2.0f;        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        this.ScaleBasedOnLoudness();

        this.MoveBasedOnPitch();
    }

    private void ScaleBasedOnLoudness()
    {
        float currentLoudness = MicrophoneManager.instance.GetRawLoudness() * this.scaleMultiplier;
        double roundedLoudness = Math.Round(currentLoudness, 2);


        //Debug.LogError("Current Loudness: " + roundedLoudness);

        this.transform.localScale = Vector3.Lerp(this.minScale, this.maxScale, (float)roundedLoudness);
    }

    private void MoveBasedOnPitch()
    {
        float currentPitch = MicrophoneManager.instance.GetNormalizedPitch();

        if (float.IsNaN(currentPitch) == true || currentPitch < 0)
        {
            return;
        }        
        
        double roundedPitch = Math.Round(currentPitch, 2);

        Debug.LogError("Pitch: " + roundedPitch);

        this.targetPosition = Vector3.Lerp(this.bottomPosition, this.topPosition, (float)roundedPitch);
        Vector3 moveDirection = this.targetPosition - this.transform.position;

        //this.transform.Translate(moveDirection.normalized * this.moveSpeed * Time.fixedDeltaTime);

        this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, this.targetPosition, this.moveSpeed * Time.fixedDeltaTime);
    }
}
