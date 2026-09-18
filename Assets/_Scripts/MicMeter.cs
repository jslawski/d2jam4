using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MicMeter : MonoBehaviour
{
    
    private Image _micImage;

    private void Awake()
    {
        this._micImage = GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    
    // Update is called once per frame
    void Update()
    {
        //Debug.LogError("Loudness: " + MicrophoneManager.instance.GetNormalizedLoudness());    
    
        this._micImage.fillAmount = MicrophoneManager.instance.GetNormalizedLoudness();

        if (this._micImage.fillAmount < 1.0f)
        {
            this._micImage.color = Color.red;
        }
        else
        {
            this._micImage.color = Color.green;
        }
    }
    
}
