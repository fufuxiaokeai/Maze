using Org.BouncyCastle.Asn1.Mozilla;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetUp : MonoBehaviour
{
    public Transform Set;
    public Transform SetButton;

    public Transform VideoButton;
    public Transform VideoBK;

    public Transform LanguageButton;
    public Transform LanguageBK;

    public AudioSource audioSource;
    public Dropdown languageDropdown;

    public Transform ExitSetButton;

    public Slider sli;
    public Text txt;

    private float volume = 1f;

    private void Awake()
    {
        Set.gameObject.SetActive(false);
        VideoBK.gameObject.SetActive(true);
        LanguageBK.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        SetButton.GetComponent<Button>().onClick.AddListener(Set_UP);
        ExitSetButton.GetComponent<Button>().onClick.AddListener(ExitSet);
        languageDropdown.options.Clear();
        languageDropdown.options.Add(new Dropdown.OptionData("简体中文"));
        languageDropdown.options.Add(new Dropdown.OptionData("英文"));
        languageDropdown.value = 0;
        sli.value = volume;
    }

    void ExitSet()
    {
        Set.gameObject.SetActive(false);
        SetButton.gameObject.SetActive(true);
    }

    void Set_UP()
    {
        Set.gameObject.SetActive(true);
        VedioSet();
        VideoButton.GetComponent<Button>().onClick.AddListener(VedioSet);
        LanguageButton.GetComponent<Button>().onClick.AddListener(LanguageSet);
    }

    void LanguageSet()
    {
        VideoBK.gameObject.SetActive(false);
        LanguageBK.gameObject.SetActive(true);
        SetButton.gameObject.SetActive(false);
        // 这里可以添加语言设置的逻辑
    }

    void VedioSet()
    {
        VideoBK.gameObject.SetActive(true);
        LanguageBK.gameObject.SetActive(false);
        SetButton.gameObject.SetActive(false);
        sli.onValueChanged.AddListener(UpdateValue);
        UpdateValue(sli.value);
    }

    void UpdateValue(float value)
    {
        audioSource.volume = value; // 更新音量
        value = value * 100f; // 将值转换为百分比
        txt.text = value.ToString("F1");

        // 在这里可以添加其他响应滑动条值变化的代码
        Debug.Log("滑动条的值: " + value);
    }

}
