using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSet : MonoBehaviour
{
    public Image MainMeun;

    public Button MainSetButton;
    public Image MainSetBK;

    public Button VideoButton;
    public Image VideoBK;

    public Button LanguageButton;
    public Image LanguageBK;

    public Button KeyPositionButton;
    public Image KeyPositionBK;

    public AudioSource audioSource;
    public Text volumeText;

    public Dropdown languageDropdown;
    public Slider volumeSlider;

    public Button ExitSetButton;

    private float volume = 1f;

    private void Awake()
    {
        volumeSlider.value = volume;
        MainSetBK.gameObject.SetActive(false);
        LanguageBK.gameObject.SetActive(true);
        languageDropdown.options.Clear();
        languageDropdown.options.Add(new Dropdown.OptionData("简体中文"));
        languageDropdown.options.Add(new Dropdown.OptionData("英文"));
        languageDropdown.value = 0;
        MainSetButton.onClick.AddListener(OpenMainSet);
    }

    void OpenMainSet()
    {
        MainMeun.gameObject.SetActive(false);
        MainSetBK.gameObject.SetActive(true);
        LanguageButton.onClick.AddListener(OpenLanguageSet);
        VideoButton.onClick.AddListener(OpenVideoSet);
        KeyPositionButton.onClick.AddListener(OpenKeyPositionSet);
        ExitSetButton.onClick.AddListener(ExitSet);
    }

    void ExitSet()
    {
        MainSetBK.gameObject.SetActive(false);
        MainMeun.gameObject.SetActive(true);
    }

    void OpenLanguageSet()
    {
        KeyPositionBK.gameObject.SetActive(false);
        VideoBK.gameObject.SetActive(false);
        LanguageBK.gameObject.SetActive(true);
    }

    void OpenVideoSet()
    {
        KeyPositionBK.gameObject.SetActive(false);
        VideoBK.gameObject.SetActive(true);
        LanguageBK.gameObject.SetActive(false);

        VideoSet();

    }

    void VideoSet()
    {
        volumeSlider.onValueChanged.AddListener(UpdateVolume);
        UpdateVolume(volumeSlider.value);
    }

    void UpdateVolume(float value)
    {
        volume = value;
        audioSource.volume = volume;
        volumeText.text = "音量：" + (volume * 100).ToString("F0") + "%";
    }

    void OpenKeyPositionSet()
    {
        KeyPositionBK.gameObject.SetActive(true);
        VideoBK.gameObject.SetActive(false);
        LanguageBK.gameObject.SetActive(false);
    }

}
