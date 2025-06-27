using Unity.VisualScripting;
using UnityEngine;
using Newtonsoft.Json;

[RequireComponent(typeof(CharacterController))]
public class GenshinCharacter : MonoBehaviour
{
    private Animator _animator;

    [Header("Movement")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float Sprint = 12f; // ��������ٶ�
    public float rotationSmooth = 0.2f;
    public float gravity = 9.82f;
    [Header("Jump")] // ������Ծ����
    public float jumpForce = 5f;

    private CharacterController _controller;
    private Transform _cameraTransform;
    private float _currentRotationVelocity;
    private float _verticalVelocity;
    private bool _isJumpPressed; // ��Ծ������

    private float networkUpdateInterval = 0.1f;
    private float lastNetworkUpdateTime = 0f;

    void Start()
    {
        SharedVariables.strength = SharedVariables.Maxstrength;
        _controller = GetComponent<CharacterController>();
        _cameraTransform = GetComponentInChildren<Camera>().transform;
        Cursor.lockState = CursorLockMode.Locked;
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (PublicVar.Isconnect || PublicVar.IsEnter || PublicVar.Isdetails || PublicVar.IsEnd || PublicVar.IsAchievement || PublicVar.IsChart || PublicVar.IsRanked || PublicVar.IsMapState)
        {
            return;
        }
        else
        {
            // ��Update�м����Ծ����
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _isJumpPressed = true;
            }
            HandleMovement();
        }
        if (transform.position.y <= -50f) // �����ɫ���䵽��������
        {
            if (PublicVar.difficulty == "��" || PublicVar.difficulty == "��ͨ")
            {
                transform.position = new Vector3(60, 0.5f, 60);
            }
            else
            {
                transform.position = new Vector3(57.5f, 0.5f, 58.5f);
            }
        }
        if (PublicVar.IsOnline)
        {
            if (Time.time - lastNetworkUpdateTime >= networkUpdateInterval)
            {
                SendPlayerInfo(transform.position, transform.rotation);
                lastNetworkUpdateTime = Time.time;
            }
        }
    }

    void SendPlayerInfo(Vector3 position, Quaternion rotation)
    {
        PlayerPosition playerPosition = new PlayerPosition
        {
            x = position.x,
            y = position.y,
            z = position.z
        };

        PlayerState playerState = new PlayerState
        {
            direct = PublicVar.direct,
            Junmp = PublicVar.Junmp,
            IsLand = PublicVar.IsLand,
        };

        RoomInfo info = new RoomInfo { roomId = RoomManager.Instance.currentRoomId };

        MessageData mess = new MessageData
        {
            type = "PlayerMove",
            token = PublicVar.token,
            position = playerPosition.ToVector3(),
            rotation = transform.rotation.eulerAngles.y,
            room = info,
            playerState = playerState
        };
        //Debug.Log(JsonConvert.SerializeObject(mess));
        NetworkManager.Instance.Send(JsonConvert.SerializeObject(mess));
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        bool isWalking = Input.GetKey(KeyCode.LeftControl); // ��������״̬

        Vector3 cameraForward = _cameraTransform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 moveDirection = (cameraForward * vertical + _cameraTransform.right * horizontal).normalized;

        // ============ ������Ծ������ ============
        if (_controller.isGrounded)
        {
            _animator.SetBool("IsLand", true); // �ӵ�ʱ���ö���״̬
            _animator.SetBool("IsMove",true);
            _animator.SetBool("IsMove-Human", false);
            PublicVar.IsLand = true;
            _verticalVelocity = -0.2f; // ��������
            if (_isJumpPressed) // �ӵ�ʱ������Ծ
            {
                _verticalVelocity = jumpForce;
                _isJumpPressed = false;
            }
        }
        else
        {
            _animator.SetBool("IsLand", false);
            _animator.SetBool("IsMove",false);
            PublicVar.IsLand = false;
            _verticalVelocity -= gravity * Time.deltaTime; // ���г�������
            _isJumpPressed = false; // ��ֹ���ж�����Ծ
        }

        // ============ �ƶ����� ============
        //float speed = isSprinting ? runSpeed : walkSpeed;
        //float sprintSpeed = isSprinting ? Sprint : walkSpeed; // ����ٶ�
        float speed;
        if (SharedVariables.strength <= 0)
        {
            speed = runSpeed;
        }
        else
        {
            if (isWalking)
            {
                speed = walkSpeed;
            }
            else if (isSprinting)
            {
                speed = Sprint;
            }
            else
            {
                speed = runSpeed;
            }
        }

        Vector3 totalMovement = moveDirection * speed + new Vector3(0, _verticalVelocity, 0);

        _controller.Move(totalMovement * Time.deltaTime);

        // ��ɫ��ת
        if (moveDirection != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetAngle,
                ref _currentRotationVelocity,
                rotationSmooth
            );
            transform.rotation = Quaternion.Euler(0, angle, 0);
        }
    }
}
