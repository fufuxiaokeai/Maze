using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MazeSet : MonoBehaviour
{
    public Button StartButton;

    public Dropdown difficulty;
    public Dropdown WmazeType;
    public Dropdown HmazeType;

    public Image mazeTypeImg;
    public Image MainSetImg;

    private void Awake()
    {
        MainSetImg.gameObject.SetActive(true);
        difficulty.options.Clear();
        difficulty.options.Add(new Dropdown.OptionData("简单"));//DFS
        difficulty.options.Add(new Dropdown.OptionData("普通"));//Prim
        difficulty.options.Add(new Dropdown.OptionData("困难"));//Kruskal
        difficulty.options.Add(new Dropdown.OptionData("地狱"));//在Kruskal的基础上增加了迷雾
        difficulty.value = 0;

        difficulty.onValueChanged.AddListener(OnDifficultyChanged);

        OnDifficultyChanged(difficulty.value);

        StartButton.onClick.AddListener(StartGame);

        if (NetworkManager.Instance == null)
        {
            GameObject networkManagerObj = new GameObject("NetworkManager");
            networkManagerObj.AddComponent<NetworkManager>();
        }
    }

    private void OnDifficultyChanged(int index)
    {
        string selectedDifficulty = difficulty.options[index].text;
        UpdateMazeSizeOptions(selectedDifficulty);
    }

    private void UpdateMazeSizeOptions(string difficultyLevel)
    {
        List<string> widthOptions = new List<string>();
        List<string> heightOptions = new List<string>();

        switch (difficultyLevel)
        {
            case "简单":
                widthOptions = new List<string> { "10", "20", "30" };
                heightOptions = new List<string> { "10", "20", "30" };
                break;
            case "普通":
                widthOptions = new List<string> { "40", "50", "60" };
                heightOptions = new List<string> { "40", "50", "60" };
                break;
            case "困难":
                widthOptions = new List<string> { "70", "80", "90", "100" };
                heightOptions = new List<string> { "70", "80", "90", "100" };
                break;
            case "地狱":
                widthOptions = new List<string> { "100", "500", "1000" };
                heightOptions = new List<string> { "100", "500", "1000" };
                break;
            default:
                widthOptions = new List<string> { "10" };
                heightOptions = new List<string> { "10" };
                break;
        }

        // 更新宽度选项
        UpdateDropdownOptions(WmazeType, widthOptions);
        // 更新高度选项
        UpdateDropdownOptions(HmazeType, heightOptions);
    }

    private void UpdateDropdownOptions(Dropdown dropdown, List<string> options)
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
        if (options.Count > 0)
        {
            dropdown.value = 0;
        }
    }

    void StartGame()
    {
        int mazeWidth = int.Parse(WmazeType.options[WmazeType.value].text);
        int mazeHeight = int.Parse(HmazeType.options[HmazeType.value].text);
        string difficultyLevel = difficulty.options[difficulty.value].text;
        PublicVar.Wmaze = mazeWidth;
        PublicVar.Hmaze = mazeHeight;
        PublicVar.difficulty = difficultyLevel;
        Debug.Log("Maze Width: " + mazeWidth + ", Maze Height: " + mazeHeight + ", Difficulty: " + difficultyLevel);
        MainSetImg.gameObject.SetActive(false);
        if (PublicVar.night)
        {
            PublicVar.IsNight = 1; // 设置夜晚模式标志
            SceneManager.LoadScene("Night mode");
        }
        else
        {
            PublicVar.IsNight = 0; // 设置白天模式标志
            SceneManager.LoadScene("SampleScene");
        }
    }

}

