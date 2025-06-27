using UnityEngine;

public class MouseControl : MonoBehaviour
{
    [Header("鼠标显隐控制")]
    public bool isMouseVisible = false; // 初始是否显示鼠标
    public KeyCode toggleKey = KeyCode.LeftAlt; // 触发键

    void Start()
    {
        UpdateCursorState();
    }

    void Update()
    {
        // 如果UI已打开，直接返回，不处理按键逻辑
        if (PublicVar.Isconnect || PublicVar.IsEnter || PublicVar.Isdetails || 
            PublicVar.IsEnd|| PublicVar.IsAchievement|| PublicVar.IsChart||
            PublicVar.IsRanked|| PublicVar.IsOnlineEnd||PublicVar.IsMapState)
        {
            isMouseVisible = true;
            UpdateCursorState();
            return;
        }

        // 检测长按左Alt键
        if (Input.GetKey(toggleKey))
        {
            SetMouseVisible(true);
        }
        else
        {
            SetMouseVisible(false);
        }
    }

    private void SetMouseVisible(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void UpdateCursorState()
    {
        SetMouseVisible(isMouseVisible);
    }
}