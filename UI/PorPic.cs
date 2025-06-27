using UnityEngine;
using UnityEngine.UI;

public class ImageChanger : MonoBehaviour
{
    // 引用UI中的Image组件
    public Image targetImage;
    public Button changeButton;

    // 存储要切换的图片数组
    public Sprite[] spritesToChange;

    // 当前显示的图片索引
    private int currentIndex = 0;

    void Start()
    {
        changeButton.onClick.AddListener(ChangeImage);
        targetImage.sprite = spritesToChange[currentIndex];
    }

    // 更换图片的方法
    public void ChangeImage()
    {
        PublicVar.image_index = currentIndex; // 更新PublicVar中的索引
        // 切换到下一张图片
        targetImage.sprite = spritesToChange[currentIndex];
        // 更新索引，循环显示
        currentIndex = Random.Range(0, spritesToChange.Length);
    }
}