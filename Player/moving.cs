using UnityEngine;
using UnityEngine.UI;

public class moving : MonoBehaviour
{
    private Animator animator;
    private bool isJumping; // 标记是否正在跳跃
    public Image img;
    public Text tet;
    public Color color;

    public AudioSource audioSource;
    public AudioClip[] Music;
    AudioClip targetClip = null;

    private float _lerpSpeed = 3f;
    private bool conditionMet = false; // 条件是否满足的标记

    void Start()
    {
        animator = GetComponent<Animator>();
        //img.enabled = false;
    }

    void Update()
    {
        //print(PublicVar.IsAchievement);
        if (PublicVar.Isconnect || PublicVar.IsEnter || PublicVar.Isdetails || PublicVar.IsEnd|| PublicVar.IsAchievement|| PublicVar.IsChart)
        {
            audioSource.Stop(); // 如果UI已打开，停止音频播放
            return; // 如果UI已打开，直接返回，不处理按键逻辑
        }
        else
        {
            // 1. 优先处理跳跃逻辑
            HandleJump();

            // 2. 如果没有在跳跃，则处理移动逻辑
            if (!isJumping)
            {
                HandleMovement();
            }
            strengthFiler();
            if (SharedVariables.strength < SharedVariables.Maxstrength)
            {
                conditionMet = true;
            }
            else conditionMet = false;
            if (conditionMet)
            {
                img.enabled = true;
                tet.enabled = true;
            }
            else
            {
                img.enabled = false;
                tet.enabled = false;
            }
            if (SharedVariables.strength <= 0) SharedVariables.strength = 0;
            if (SharedVariables.strength >= SharedVariables.Maxstrength) SharedVariables.strength = SharedVariables.Maxstrength;
            // 统一处理音频播放
            if (targetClip != null && targetClip != audioSource.clip)
            {
                audioSource.clip = targetClip;
                audioSource.Play();
            }
            else if (!audioSource.isPlaying && targetClip != null)
            {
                audioSource.Play();
            }
            float perc = SharedVariables.strength / SharedVariables.Maxstrength;
            int str = (int)SharedVariables.strength;
            tet.text = str + "/" + SharedVariables.Maxstrength;
            color = (perc <= 0.4f) ? Color.red : Color.yellow;
            img.color = color;
        }
    }

    void HandleJump()
    {
        // 按下空格键触发跳跃
        if (Input.GetKeyDown(KeyCode.Space))
        {
            audioSource.Stop();
            isJumping = true;
            animator.SetBool("IsJump", true);

            // 跳跃时强制重置移动状态参数
            animator.SetBool("IsMove", false);
            animator.SetBool("IsWalk", false);
            animator.SetBool("IsRun", false);
            animator.SetBool("IsSprint", false);
            animator.SetBool("IsMove-Jump", true);
            animator.SetBool("IsMove-Human", false);
            PublicVar.Junmp = "IsJump";
        }

        // 松开空格键结束跳跃
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
            animator.SetBool("IsJump", false);
            animator.SetBool("IsMove-Jump", false);
            animator.SetBool("IsMove-Human", true);
            PublicVar.Junmp = "";
        }
        SharedVariables.strength += SharedVariables.staminaRecoveryRate * Time.deltaTime;
    }

    void HandleMovement()
    {
        // 检测移动输入
        bool isMoving = Input.GetKey(KeyCode.W) ||
                        Input.GetKey(KeyCode.A) ||
                        Input.GetKey(KeyCode.S) ||
                        Input.GetKey(KeyCode.D);
        animator.SetBool("IsMove", isMoving);

        if (isMoving)
        {
            // 优先级：冲刺 > 行走 > 奔跑
            bool isSprinting = Input.GetKey(KeyCode.LeftShift);
            bool isWalking = Input.GetKey(KeyCode.LeftControl);

            if (isSprinting)
            {
                targetClip = Music[0];
                animator.SetBool("IsSprint", true);
                animator.SetBool("IsWalk", false);
                animator.SetBool("IsRun", false);
                PublicVar.direct = "IsSprint";
                // 体力消耗
                SharedVariables.strength -= SharedVariables.staminaDepleteRate * Time.deltaTime;
                if(SharedVariables.strength <= 0)
                { 
                    animator.SetBool("IsSprint", false);
                    animator.SetBool("IsWalk", false);
                    animator.SetBool("IsRun", true);
                    PublicVar.direct = "IsRun";
                }
            }
            else if (isWalking)
            {
                targetClip = Music[1];
                animator.SetBool("IsWalk", true);
                animator.SetBool("IsSprint", false);
                animator.SetBool("IsRun", false);
                PublicVar.direct = "IsWalk";
                // 体力恢复
                SharedVariables.strength += SharedVariables.staminaRecoveryRate * Time.deltaTime;
            }
            else
            {
                targetClip = Music[0];
                animator.SetBool("IsRun", true);
                animator.SetBool("IsWalk", false);
                animator.SetBool("IsSprint", false);
                PublicVar.direct = "IsRun";
                SharedVariables.strength += SharedVariables.staminaRecoveryRate * Time.deltaTime;
            }
        }
        else
        {
            audioSource.Stop(); // 停止音乐播放
            // 停止移动时重置所有状态
            animator.SetBool("IsRun", false);
            animator.SetBool("IsWalk", false);
            animator.SetBool("IsSprint", false);
            PublicVar.direct = "";
            SharedVariables.strength += SharedVariables.staminaRecoveryRate * Time.deltaTime;
        }
    }

    private void strengthFiler()
    {
        img.fillAmount = Mathf.Lerp(img.fillAmount, SharedVariables.strength / SharedVariables.Maxstrength, Time.deltaTime * _lerpSpeed);
    }

}