using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchController : MonoBehaviour
{
    public Toggle toggle1;
    public Toggle toggle2;

    void Start()
    {
        toggle1.onValueChanged.AddListener(OnToggleValueChanged);
        toggle2.onValueChanged.AddListener(OnTimeChanged);
    }

    void OnTimeChanged(bool isOn)
    {
        if(isOn)
        {
            PublicVar.time = true;
            PublicVar.Is_challenge = 1;
        }
        else
        {
            PublicVar.time = false;
            PublicVar.Is_challenge = 1;
        }
    }

    void OnToggleValueChanged(bool isOn)
    {
        if(isOn)
        {
            PublicVar.night = true;
        }
        else
        {
            PublicVar.night = false;
        }
    }

}
