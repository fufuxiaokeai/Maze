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
    public float Sprint = 12f; // 新增冲刺速度
    public float rotationSmooth = 0.2f;
    public float gravity = 9.82f;
    [Header("Jump")] // 新增跳跃参数
    public float jumpForce = 5f;

    private CharacterController _controller;
    private Transform _cameraTransform;
    private float _currentRotationVelocity;
    private float _verticalVelocity;
    private bool _isJumpPressed; // 跳跃输入标记

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
            // 在Update中检测跳跃输入
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _isJumpPressed = true;
            }
            HandleMovement();
        }
        if (transform.position.y <= -50f) // 如果角色掉落到地面以下
        {
            if (PublicVar.difficulty == "简单" || PublicVar.difficulty == "普通")
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
        bool isWalking = Input.GetKey(KeyCode.LeftControl); // 新增行走状态

        Vector3 cameraForward = _cameraTransform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 moveDirection = (cameraForward * vertical + _cameraTransform.right * horizontal).normalized;

        // ============ 处理跳跃和重力 ============
        if (_controller.isGrounded)
        {
            _animator.SetBool("IsLand", true); // 接地时设置动画状态
            _animator.SetBool("IsMove",true);
            _animator.SetBool("IsMove-Human", false);
            PublicVar.IsLand = true;
            _verticalVelocity = -0.2f; // 保持贴地
            if (_isJumpPressed) // 接地时允许跳跃
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
            _verticalVelocity -= gravity * Time.deltaTime; // 空中持续下落
            _isJumpPressed = false; // 防止空中二次跳跃
        }

        // ============ 移动计算 ============
        //float speed = isSprinting ? runSpeed : walkSpeed;
        //float sprintSpeed = isSprinting ? Sprint : walkSpeed; // 冲刺速度
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

        // 角色旋转
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
