using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DeviceSelector : MonoBehaviour
{    
    private TMP_Dropdown _dropdownMenu;

    private void Awake()
    {
        this._dropdownMenu = GetComponent<TMP_Dropdown>();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.PopulateDropdown();    
    }

    private void PopulateDropdown()
    {
        this._dropdownMenu.AddOptions(Microphone.devices.ToList());
    }

    public void UpdateDevice()
    {
        GameOptions.device = this._dropdownMenu.options[this._dropdownMenu.value].text;
        MicrophoneManager.instance.Refresh();
    }
}
