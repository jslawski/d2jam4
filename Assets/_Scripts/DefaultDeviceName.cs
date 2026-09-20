using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DefaultDeviceName : MonoBehaviour
{
    private TextMeshProUGUI _deviceName;    

    void Start()
    {
        this._deviceName = GetComponent<TextMeshProUGUI>();
        this._deviceName.text = Microphone.devices[0];
    }
}
