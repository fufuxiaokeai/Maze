using Google.Protobuf.WellKnownTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaUISet : MonoBehaviour
{
    public Button SetButton;

    public Button LaugButton;
    public Button ExitButton;
    public Button VedioButton;

    public Image SetImage;
    public Image LaugImage;
    public Image VedioImage;

    public Button recordButton;
    public Button chartsButton;
    public Button achievementButton;
    public Button ChatButton;

    public Slider slider;
    public Dropdown languageDropdown;

    public Text txt;
    public AudioSource audioSource;

    public Toggle night;
    public Toggle time;
    public Image WallImg;
    public Image GroundImg;

    private float volume = 1f;

    private void Awake()
    {
        night.gameObject.SetActive(true);
        time.gameObject.SetActive(true);
        WallImg.gameObject.SetActive(true);
        GroundImg.gameObject.SetActive(true);
        slider.value = volume;
        languageDropdown.options.Clear();
        languageDropdown.options.Add(new Dropdown.OptionData("��������"));
        languageDropdown.options.Add(new Dropdown.OptionData("Ӣ��"));
        languageDropdown.value = 0;
        SetButton.onClick.AddListener(Set);
        Cursor.visible = true; // ��ʾ���
        Cursor.lockState = CursorLockMode.None; // �������
    }

    private void Set()
    {
        Vedio();
        night.gameObject.SetActive(false);
        time.gameObject.SetActive(false);
        WallImg.gameObject.SetActive(false);
        GroundImg.gameObject.SetActive(false);
        SetImage.gameObject.SetActive(true);
        recordButton.gameObject.SetActive(false);
        chartsButton.gameObject.SetActive(false);
        achievementButton.gameObject.SetActive(false);
        ChatButton.gameObject.SetActive(false);
        LaugButton.onClick.AddListener(Laug);
        ExitButton.onClick.AddListener(Exit);
        VedioButton.onClick.AddListener(Vedio);
    }

    private void Laug()
    {
        LaugImage.gameObject.SetActive(true);
        VedioImage.gameObject.SetActive(false);
    }

    private void Vedio()
    {
        VedioImage.gameObject.SetActive(true);
        LaugImage.gameObject.SetActive(false);
        slider.onValueChanged.AddListener(UpdateValue);
        UpdateValue(slider.value);
    }

    void UpdateValue(float value)
    {
        audioSource.volume = value; // ��������
        value = value * 100f; // ��ֵת��Ϊ�ٷֱ�
        txt.text = value.ToString("F1");

        // ������������������Ӧ������ֵ�仯�Ĵ���
        Debug.Log("��������ֵ: " + value);
    }

    private void Exit()
    {
        SetImage.gameObject.SetActive(false);
        recordButton.gameObject.SetActive(true);
        chartsButton.gameObject.SetActive(true);
        achievementButton.gameObject.SetActive(true);
        ChatButton.gameObject.SetActive(true);
        night.gameObject.SetActive(true);
        time.gameObject.SetActive(true);
        WallImg.gameObject.SetActive(true);
        GroundImg.gameObject.SetActive(true);
    }

}
