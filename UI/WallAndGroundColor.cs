using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WallAndGroundColor : MonoBehaviour
{
    public Image WallColor;
    public Image GroundColor;

    public Slider WR;
    public Slider WG;
    public Slider WB;

    public Slider GR;
    public Slider GG;
    public Slider GB;

    public Material WallMaterial;
    public Material GroundMaterial;

    private void Awake()
    {
        WR.value = PublicVar.WR;
        WG.value = PublicVar.WG;
        WB.value = PublicVar.WB;

        GR.value = PublicVar.GR;
        GG.value = PublicVar.GG;
        GB.value = PublicVar.GB;
    }

    private void Start()
    {
        WG.onValueChanged.AddListener(UpdateWallColor);
        WB.onValueChanged.AddListener(UpdateWallColor);
        WR.onValueChanged.AddListener(UpdateWallColor);
        UpdateWallColor(WR.value); // 初始化墙壁颜色

        GR.onValueChanged.AddListener(UpdateGroundColor);
        GG.onValueChanged.AddListener(UpdateGroundColor);
        GB.onValueChanged.AddListener(UpdateGroundColor);
        UpdateGroundColor(GR.value); // 初始化地面颜色
    }

    private void UpdateWallColor(float value)
    {
        PublicVar.WR = WR.value;
        PublicVar.WG = WG.value;
        PublicVar.WB = WB.value;
        WallColor.color = new Color(value, WG.value, WB.value);
        WallMaterial.color = WallColor.color;
    }

    private void UpdateGroundColor(float value)
    {
        PublicVar.GR = GR.value;
        PublicVar.GG = GG.value;
        PublicVar.GB = GB.value;
        GroundColor.color = new Color(value, GG.value, GB.value);
        GroundMaterial.color = GroundColor.color;
    }

}
