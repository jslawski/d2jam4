using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScaler : MonoBehaviour
{
    private float scaleMultiplier = 10;

    public Vector3 minScale;
    public Vector3 maxScale;

    // Start is called before the first frame update
    void Start()
    {
        this.minScale = Vector3.one * 0.5f;
        this.maxScale = Vector3.one * 2.0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float currentLoudness = MicrophoneManager.instance.GetRawLoudness() * this.scaleMultiplier;
        double roundedLoudness = Math.Round(currentLoudness, 2);


        Debug.LogError("Current Loudness: " + roundedLoudness);

        this.transform.localScale = Vector3.Lerp(this.minScale, this.maxScale, (float)roundedLoudness);
    }
}
