using UnityEngine;
using UnityEngine.UI;

public class moving : MonoBehaviour
{
    private Animator animator;
    private bool isJumping; // ����Ƿ�������Ծ
    public Image img;
    public Text tet;
    public Color color;

    public AudioSource audioSource;
    public AudioClip[] Music;
    AudioClip targetClip = null;

    private float _lerpSpeed = 3f;
    private bool conditionMet = false; // �����Ƿ�����ı��

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
            audioSource.Stop(); // ���UI�Ѵ򿪣�ֹͣ��Ƶ����
            return; // ���UI�Ѵ򿪣�ֱ�ӷ��أ����������߼�
        }
        else
        {
            // 1. ���ȴ�����Ծ�߼�
            HandleJump();

            // 2. ���û������Ծ�������ƶ��߼�
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
            // ͳһ������Ƶ����
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
        // ���¿ո��������Ծ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            audioSource.Stop();
            isJumping = true;
            animator.SetBool("IsJump", true);

            // ��Ծʱǿ�������ƶ�״̬����
            animator.SetBool("IsMove", false);
            animator.SetBool("IsWalk", false);
            animator.SetBool("IsRun", false);
            animator.SetBool("IsSprint", false);
            animator.SetBool("IsMove-Jump", true);
            animator.SetBool("IsMove-Human", false);
            PublicVar.Junmp = "IsJump";
        }

        // �ɿ��ո��������Ծ
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
        // ����ƶ�����
        bool isMoving = Input.GetKey(KeyCode.W) ||
                        Input.GetKey(KeyCode.A) ||
                        Input.GetKey(KeyCode.S) ||
                        Input.GetKey(KeyCode.D);
        animator.SetBool("IsMove", isMoving);

        if (isMoving)
        {
            // ���ȼ������ > ���� > ����
            bool isSprinting = Input.GetKey(KeyCode.LeftShift);
            bool isWalking = Input.GetKey(KeyCode.LeftControl);

            if (isSprinting)
            {
                targetClip = Music[0];
                animator.SetBool("IsSprint", true);
                animator.SetBool("IsWalk", false);
                animator.SetBool("IsRun", false);
                PublicVar.direct = "IsSprint";
                // ��������
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
                // �����ָ�
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
            audioSource.Stop(); // ֹͣ���ֲ���
            // ֹͣ�ƶ�ʱ��������״̬
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