using UnityEngine;
using UnityEngine.UI;

public class ImageChanger : MonoBehaviour
{
    // ����UI�е�Image���
    public Image targetImage;
    public Button changeButton;

    // �洢Ҫ�л���ͼƬ����
    public Sprite[] spritesToChange;

    // ��ǰ��ʾ��ͼƬ����
    private int currentIndex = 0;

    void Start()
    {
        changeButton.onClick.AddListener(ChangeImage);
        targetImage.sprite = spritesToChange[currentIndex];
    }

    // ����ͼƬ�ķ���
    public void ChangeImage()
    {
        PublicVar.image_index = currentIndex; // ����PublicVar�е�����
        // �л�����һ��ͼƬ
        targetImage.sprite = spritesToChange[currentIndex];
        // ����������ѭ����ʾ
        currentIndex = Random.Range(0, spritesToChange.Length);
    }
}