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
        languageDropdown.options.Add(new Dropdown.OptionData("��������"));
        languageDropdown.options.Add(new Dropdown.OptionData("Ӣ��"));
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
        // �����������������õ��߼�
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
        audioSource.volume = value; // ��������
        value = value * 100f; // ��ֵת��Ϊ�ٷֱ�
        txt.text = value.ToString("F1");

        // ������������������Ӧ������ֵ�仯�Ĵ���
        Debug.Log("��������ֵ: " + value);
    }

}
