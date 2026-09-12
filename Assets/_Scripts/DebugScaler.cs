using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScaler : MonoBehaviour
{
    private float scaleMultiplier = 3;

    public Vector3 minScale;
    public Vector3 maxScale;

    private Vector3 _targetScale;

    public Vector3 topPosition;
    public Vector3 bottomPosition;

    public Vector3 targetPosition = Vector3.zero;
    public float moveSpeed = 10.0f;

    public float scaleSpeed = 1.0f;

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

        this._targetScale = Vector3.Lerp(this.minScale, this.maxScale, (float)roundedLoudness);

        //Debug.LogError("Current Loudness: " + roundedLoudness);

        this.transform.localScale = Vector3.Lerp(this.transform.localScale, this._targetScale, this.scaleSpeed * Time.fixedDeltaTime);
    }

    private void MoveBasedOnPitch()
    {
        float currentPitch = MicrophoneManager.instance.GetCurrentNormalizedPitch();

        if (float.IsNaN(currentPitch) == true || currentPitch < 0)
        {
            this.targetPosition = this.transform.localPosition;
            return;
        }        
        
        double roundedPitch = Math.Round(currentPitch, 2);

        //Debug.LogError("Raw Pitch: " + MicrophoneManager.instance.GetRawPitch() + "\nNormalized Pitch: " + currentPitch);

        this.targetPosition = Vector3.Lerp(this.bottomPosition, this.topPosition, (float)roundedPitch);
        Vector3 moveDirection = this.targetPosition - this.transform.position;

        this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, this.targetPosition, this.moveSpeed * Time.fixedDeltaTime);
    }
}
