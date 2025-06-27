using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Agreement : MonoBehaviour
{
    public Button OpenAgreeButton;
    public Button CloseAgreeButton;
    public Image AgreementImage;

    private void Awake()
    {
        AgreementImage.gameObject.SetActive(false);
        OpenAgreeButton.onClick.AddListener(()=> AgreementImage.gameObject.SetActive(true));
        CloseAgreeButton.onClick.AddListener(() => AgreementImage.gameObject.SetActive(false));
    }
}
