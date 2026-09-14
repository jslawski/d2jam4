using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeMultiplierOption : MonoBehaviour
{
    [SerializeField]
    private Slider _slider;

    [SerializeField]
    private Image _micImage;

    [SerializeField]
    private TextMeshProUGUI _valueText;
    /*
    // Update is called once per frame
    void Update()
    {
        this._micImage.fillAmount = MicrophoneManager.instance.GetNormalizedLoudness();

        if (this._micImage.fillAmount < 0.8f)
        {
            this._micImage.color = Color.red;
        }
        else
        {
            this._micImage.color = Color.green;
        }
    }
    */
    public void OnValueUpdate()
    {
        GameOptions.volumeMultiplier = this._slider.value;

        float roundedValue = (float)System.Math.Round(this._slider.value, 1);

        //this._valueText.text = roundedValue.ToString();
    }
}
