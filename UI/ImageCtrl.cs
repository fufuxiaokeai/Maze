using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageCtrl : MonoBehaviour
{
    public Image Selected;
    public Button button;
    void Start()
    {
        Selected.gameObject.SetActive(false);
        button.onClick.AddListener(() => {
            if (Selected.gameObject.activeSelf)
            {
                Selected.gameObject.SetActive(false);
            }
            else
            {
                Selected.gameObject.SetActive(true);
            }
        });
    }
}
