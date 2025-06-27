using UnityEngine;

public class MouseControl : MonoBehaviour
{
    [Header("�����������")]
    public bool isMouseVisible = false; // ��ʼ�Ƿ���ʾ���
    public KeyCode toggleKey = KeyCode.LeftAlt; // ������

    void Start()
    {
        UpdateCursorState();
    }

    void Update()
    {
        // ���UI�Ѵ򿪣�ֱ�ӷ��أ����������߼�
        if (PublicVar.Isconnect || PublicVar.IsEnter || PublicVar.Isdetails || 
            PublicVar.IsEnd|| PublicVar.IsAchievement|| PublicVar.IsChart||
            PublicVar.IsRanked|| PublicVar.IsOnlineEnd||PublicVar.IsMapState)
        {
            isMouseVisible = true;
            UpdateCursorState();
            return;
        }

        // ��ⳤ����Alt��
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