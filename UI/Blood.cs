using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blood : MonoBehaviour
{
    public Image image;
    public Text text;
    public Color color;
    public GameObject player;
    public float health = 100f, maxHealth = 100f;
    private float _lerpSpeed = 3f;
    private bool hasFallen = false;
    private float fallStartY; // ��¼��������Y����
    private float fallResetDelay = 0.8f; // �ӳ�ʱ�䣬�ɸ���ʵ���������
    private bool isFalling = false; // ��ǽ�ɫ�Ƿ����ڵ���
    private float lastFallTime;
    private CharacterController characterController;

    // Start is called before the first frame update
    void Start()
    {
        characterController = player.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        BarFiler();
        fall();
        HandleFallDamage();
        float percentage = health / maxHealth;
        text.text = health + "/" + maxHealth;
        if (health<=0) health = 0;
        if (percentage <= 0.2f)
        {
            color = Color.red;
            image.color = color;
        }
        else if (percentage > 0.2f && percentage <= 0.4f)
        {
            color = Color.yellow;
            image.color = color;
        }
        else
        {
            color = Color.green;
            image.color = color;
        }
    }

    public void BarFiler()
    {
        image.fillAmount = Mathf.Lerp(image.fillAmount, health / maxHealth, Time.deltaTime * _lerpSpeed);
    }

    private void fall()
    {
        if (player.transform.position.y <= -49f && !hasFallen)
        {
            health -= 15;
            hasFallen = true;
            lastFallTime = Time.time;
        }
        else if (Time.time - lastFallTime > fallResetDelay && player.transform.position.y > -50f)
        {
            hasFallen = false;
        }
    }

    private void HandleFallDamage()
    {
        if (characterController.isGrounded)
        {
            if (isFalling)
            {
                float fallDistance = fallStartY - player.transform.position.y;
                ApplyFallDamage(fallDistance);
                isFalling = false;
            }
        }
        else
        {
            if (!isFalling)
            {
                fallStartY = player.transform.position.y;
                isFalling = true;
            }
        }
    }

    private void ApplyFallDamage(float fallDistance)
    {
        if (fallDistance > 30f)
        {
            health = 0;
        }
        else if (fallDistance > 20f)
        {
            health -= 50;
        }
        else if (fallDistance > 10f)
        {
            health -= 5;
        }
    }

}
